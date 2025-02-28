using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;
using QuickTalk.Api.Models;
using QuickTalk.Api.Repositories.Interfaces;

namespace QuickTalk.Api.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ChatDbContext _context;

        public AuthRepository(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetRefreshTokenByUserIdAsync(int userId)
        {
            return await _context.RefreshTokens
                .SingleOrDefaultAsync(rt => rt.UserID == userId);
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .SingleOrDefaultAsync(rt => rt.Token == token);
        }

        public void UpdateRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
        }

        public void RemoveRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
        }
    }
}