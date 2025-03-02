using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;

namespace QuickTalk.Api.Repositories.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<int> GetChatroomByMessageIdAsync(int messageId);
        Task<string> GetLastMessageForChatroom(int chatroomId);
    }
}
