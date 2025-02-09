using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;
using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;
using QuickTalk.Api.Repositories.Interfaces;

namespace QuickTalk.Api.Repositories.Implementations
{
    public class ChatroomsRepository : GenericRepository<Chatroom>, IChatroomsRepository
    {
        public ChatroomsRepository(ChatDbContext context) : base(context)
        {
        }

        public async Task<Chatroom?> GetDirectChatroomAsync(int senderId, int recipientId)
        {
            return await _dbSet
                .Include(cr => cr.UserChatrooms)
                .FirstOrDefaultAsync(cr =>
                    cr.RoomType != ChatroomType.Group &&
                    cr.UserChatrooms.Count(uc => uc.UserID == senderId || uc.UserID == recipientId) == 2
                );
        }

        public async Task<Chatroom?> GetChatroomDataByIdAsync(int chatroomId)
        {
            return await _dbSet.Include(c => c.UserChatrooms)
                .ThenInclude(uc => uc.User).Include(c => c.Messages)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(c => c.ChatroomID == chatroomId);
        }

        public async Task<List<Chatroom>> GetUserChatroomsAsync(int userId, ChatroomType roomType)
        {
            return await _dbSet
                .Include(c => c.UserChatrooms)
                .ThenInclude(uc => uc.User)
                .Include(c => c.Messages)
                .Where(c => c.UserChatrooms.Any(uc => uc.UserID == userId) &&
                            (
                                (roomType == ChatroomType.Group) ?
                                (c.RoomType == ChatroomType.Group) :
                                (c.RoomType == ChatroomType.Direct && c.Messages.Any())
                            ))
                .ToListAsync();
        }

        public async Task<bool> IsChatroomTypeGroup(int chatroomId)
        {
            var chatroom = await _dbSet.FirstOrDefaultAsync(c => c.ChatroomID == chatroomId);
            if (chatroom == null)
                return false;

            if (chatroom.RoomType == ChatroomType.Direct)
                return false;

            return true;
        }
    }
}
