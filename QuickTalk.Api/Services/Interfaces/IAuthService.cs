using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;
using QuickTalk.Api.Repositories.Interfaces;

namespace QuickTalk.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginModel loginModel);
        Task<string> RegisterAsync(RegisterModel registerModel);
    }
}
