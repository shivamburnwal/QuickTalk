using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;
using QuickTalk.Api.Models;
using QuickTalk.Api.Repositories.Interfaces;

namespace QuickTalk.Api.Repositories.Implementations
{
    public class UsersRepository : GenericRepository<User>, IUsersRepository
    {
        public UsersRepository(ChatDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            return await _dbSet.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> SearchUsersByNameAsync(string name)
        {
            return await _dbSet
                .Where(u => u.Username.Contains(name))
                .ToListAsync();
        }

        public async Task<User?> GetUserWithChatroomsAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.UserChatrooms)
                .ThenInclude(uc => uc.Chatroom)
                .FirstOrDefaultAsync(u => u.UserID == userId);
        }
    }
}
