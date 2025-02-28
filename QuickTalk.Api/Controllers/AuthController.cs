using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickTalk.Api.DTOs;
using QuickTalk.Api.Services.Interfaces;
using QuickTalk.Api.Services.Others;

namespace QuickTalk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly AuthorizationService _authorizationService;

        public AuthController(IAuthService authService, AuthorizationService authorizationService)
        {
            _authService = authService;
            _authorizationService = authorizationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                AuthResponse response = await _authService.LoginAsync(loginModel);

                // Set Refresh Token as HttpOnly Cookie instead of sending to Frontend.
                Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(1)
                });

                return Ok(new { token = response.AccessToken });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid username or password.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            try
            {
                var message = await _authService.RegisterAsync(registerModel);
                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                // Read refresh token from HttpOnly Cookie
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized("Refresh token missing.");

                var response = await _authService.RefreshTokenAsync(refreshToken);

                // Set new refresh token
                Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(1)
                });

                return Ok(new { token = response.AccessToken });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var message = await _authService.LogoutAsync();
                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}