using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;
using QuickTalk.Api.Models;
using QuickTalk.Api.Repositories.Interfaces;

namespace QuickTalk.Api.Repositories.Implementations
{
    public class UserChatroomsRepository : GenericRepository<UserChatroom>, IUserChatroomsRepository
    {
        public UserChatroomsRepository(ChatDbContext context) : base(context) { }

        public async Task AddUserToChatroomAsync(int chatroomId, int userId)
        {
            var userChatroom = new UserChatroom
            {
                UserID = userId,
                ChatroomID = chatroomId,
                Role = ChatroomRole.Member
            };
            await _dbSet.AddAsync(userChatroom);
            return;
        }

        public async Task RemoveUserFromChatroom(int chatroomId, int userId)
        {
            var userChatroom = await _dbSet.FirstOrDefaultAsync(uc => uc.ChatroomID == chatroomId && uc.UserID == userId);
            
            if (userChatroom != null)
                _dbSet.Remove(userChatroom);
            return;
        }

        public async Task<UserChatroom?> IsUserInChatroomAsync(int chatroomId, int userId)
        {
            return await _dbSet.FirstOrDefaultAsync(uc => uc.ChatroomID == chatroomId && uc.UserID == userId);
        }

        public async Task<bool> CanUserLeaveChatroom(int chatroomId, int userId)
        {
            var userChatroom = await _dbSet
                .FirstOrDefaultAsync(uc => uc.ChatroomID == chatroomId && uc.UserID == userId);

            if (userChatroom == null)
                return false;

            var adminCount = await _dbSet
                .Where(uc => uc.ChatroomID == chatroomId && uc.Role == ChatroomRole.Admin)
                .CountAsync();

            var totalUserCount = await _dbSet
                .Where(uc => uc.ChatroomID == chatroomId)
                .CountAsync();

            // Cannot leave as the last admin with other users
            if (userChatroom.Role == ChatroomRole.Admin) {
                if (adminCount == 1 && totalUserCount > 1)
                    return false;
            }
            return true;
        }

    }
}
