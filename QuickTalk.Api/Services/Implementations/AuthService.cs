using Microsoft.IdentityModel.Tokens;
using QuickTalk.Api.Data.Interfaces;
using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;
using QuickTalk.Api.Services.Interfaces;
using QuickTalk.Api.Services.Others;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuickTalk.Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(LoginModel loginModel)
        {
            var user = await _unitOfWork.UsersRepository.GetUserByUsernameAsync(loginModel.Username);
            if (user == null || !PasswordService.VerifyPassword(loginModel.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid username or password.");

            // Update LastLogin property
            user.LastLogin = DateTime.UtcNow;
            _unitOfWork.UsersRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return GenerateJwtToken(user);
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

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is not configured.");
            var encodedKey = Encoding.UTF8.GetBytes(key);

            // create token.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(encodedKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
