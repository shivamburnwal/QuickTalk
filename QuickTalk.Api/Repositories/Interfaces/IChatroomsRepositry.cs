using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;

namespace QuickTalk.Api.Repositories.Interfaces
{
    public interface IChatroomsRepository : IGenericRepository<Chatroom>
    {
        Task<Chatroom?> GetDirectChatroomAsync(int senderId, int recipientId);
        Task<Chatroom?> GetChatroomDataByIdAsync(int chatroomId);
        Task<List<Chatroom>> GetUserChatroomsAsync(int userId, ChatroomType roomType);
        Task<bool> IsChatroomTypeGroup(int chatroomId);
    }
}
