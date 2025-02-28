using QuickTalk.Api.Models;

namespace QuickTalk.Api.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<RefreshToken?> GetRefreshTokenByUserIdAsync(int userId);
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        void UpdateRefreshToken(RefreshToken refreshToken);
        void RemoveRefreshToken(RefreshToken refreshToken);
    }
}
