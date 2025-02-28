using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;
using QuickTalk.Api.Repositories.Interfaces;

namespace QuickTalk.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginModel loginModel);
        Task<string> RegisterAsync(RegisterModel registerModel);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task<string> LogoutAsync();
    }
}
