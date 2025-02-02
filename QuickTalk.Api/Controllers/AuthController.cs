using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuickTalk.Api.Data;
using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;
using QuickTalk.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuickTalk.Api.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ChatDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ChatDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = _context.Users.SingleOrDefault(x => x.Email == login.Username);
            if (user == null || !PasswordService.VerifyPassword(login.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            // Generate token for user.
            var token = GenerateToken(user);
            return Ok(new { Token = token });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            // Check if the username or email is already taken
            if (_context.Users.Any(u => u.Username == registerModel.Username))
            {
                return BadRequest("Username is already taken.");
            }

            if (_context.Users.Any(u => u.Email == registerModel.Email))
            {
                return BadRequest("Email is already registered.");
            }

            // Hash the password
            var passwordHash = PasswordService.HashPassword(registerModel.Password);

            // Create a new user
            var user = new User
            {
                Username = registerModel.Username,
                Email = registerModel.Email,
                PasswordHash = passwordHash,
                Role = "User"
            };

            // Add the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully." });
        }


        private string GenerateToken(User user)
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