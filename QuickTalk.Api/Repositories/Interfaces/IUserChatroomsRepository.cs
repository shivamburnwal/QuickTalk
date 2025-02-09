using QuickTalk.Api.Models;

namespace QuickTalk.Api.Repositories.Interfaces
{
    public interface IUserChatroomsRepository : IGenericRepository<UserChatroom>
    {
        Task AddUserToChatroomAsync(int chatroomId, int userId);
        Task<UserChatroom?> IsUserInChatroomAsync(int chatroomId, int userId);
        Task<bool> CanUserLeaveChatroom(int chatroomId, int userId);
    }
}
