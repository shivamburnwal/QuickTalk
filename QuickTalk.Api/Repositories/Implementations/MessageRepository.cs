using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;
using QuickTalk.Api.Models;
using QuickTalk.Api.Repositories.Interfaces;

namespace QuickTalk.Api.Repositories.Implementations
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(ChatDbContext context) : base(context)
        {
        }

        public async Task<int> GetChatroomByMessageIdAsync(int messageId)
        {
            return await _dbSet
                .Where(m => m.MessageID == messageId)
                .Select(m => m.ChatroomID)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetLastMessageForChatroom(int chatroomId)
        {
            var lastMessage = await _dbSet
                .Where(m => m.ChatroomID == chatroomId)
                .OrderByDescending(m => m.SentAt)
                .Select(m => m.Content)
                .FirstOrDefaultAsync();

            return lastMessage ?? "";
        }
    }
}