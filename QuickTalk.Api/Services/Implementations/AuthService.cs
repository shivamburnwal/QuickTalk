using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuickTalk.Api.Data.Interfaces;
using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;
using QuickTalk.Api.Services.Interfaces;
using QuickTalk.Api.Services.Others;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QuickTalk.Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly AuthorizationService _authorizationService;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, 
            AuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _authorizationService = authorizationService;
        }

        public async Task<AuthResponse> LoginAsync(LoginModel loginModel)
        {
            var user = await _unitOfWork.UsersRepository.GetUserByEmailAsync(loginModel.Email);
            if (user == null || !PasswordService.VerifyPassword(loginModel.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid username or password.");

            // Update LastLogin property
            user.LastLogin = DateTime.UtcNow;
            _unitOfWork.UsersRepository.Update(user);

            // Generate tokens.
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Check if a refresh token already exists for the user
            var existingRefreshToken = await _unitOfWork.AuthRepository.GetRefreshTokenByUserIdAsync(user.UserID);

            if (existingRefreshToken != null)
            {
                existingRefreshToken.Token = refreshToken;
                existingRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(1);
                _unitOfWork.AuthRepository.UpdateRefreshToken(existingRefreshToken);
            }
            else
            {
                RefreshToken refreshTokenEntity = new RefreshToken
                {
                    UserID = user.UserID,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(1)
                };
                await _unitOfWork.AuthRepository.SaveRefreshTokenAsync(refreshTokenEntity);
            }
            await _unitOfWork.SaveChangesAsync();

            // Return response containing both tokens.
            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<string> RegisterAsync(RegisterModel registerModel)
        {
            // Check if the username or email is already taken
            if (await _unitOfWork.UsersRepository.IsUsernameTakenAsync(registerModel.Username))
                throw new Exception("Username is already taken.");

            if (await _unitOfWork.UsersRepository.IsEmailTakenAsync(registerModel.Email))
                throw new Exception("Email is already registered.");

            // Hash the password
            var passwordHash = PasswordService.HashPassword(registerModel.Password);

            // Create a new user
            var user = new User {
                Username = registerModel.Username,
                Email = registerModel.Email,
                PasswordHash = passwordHash,
                Role = "User"
            };

            await _unitOfWork.UsersRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return "User registered successfully.";
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshTokenStr)
        {
            var refreshToken = await _unitOfWork.AuthRepository.GetRefreshTokenAsync(refreshTokenStr);

            // Check if refresh token exists and is valid
            if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var user = await _unitOfWork.UsersRepository.GetByIdAsync(refreshToken.UserID);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            // Generate new tokens
            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Remove old refresh token
            _unitOfWork.AuthRepository.RemoveRefreshToken(refreshToken);

            // Save the new refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                UserID = user.UserID,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };
            await _unitOfWork.AuthRepository.SaveRefreshTokenAsync(newRefreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<string> LogoutAsync()
        {
            var userId = _authorizationService.GetAuthenticatedUserId();
            if (userId == null)
                throw new Exception("User must be logged in.");

            var refreshToken = await _unitOfWork.AuthRepository.GetRefreshTokenByUserIdAsync(userId.Value);

            if (refreshToken == null)
                throw new UnauthorizedAccessException("No active session found.");

            _unitOfWork.AuthRepository.RemoveRefreshToken(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return "User logged out successfully.";
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is not configured.");
            var encodedKey = Encoding.UTF8.GetBytes(key);

            var expiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes");

            // create token.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                    }),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(encodedKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
