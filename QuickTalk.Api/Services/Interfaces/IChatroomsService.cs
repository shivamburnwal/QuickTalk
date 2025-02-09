using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;

namespace QuickTalk.Api.Services.Interfaces
{
    public interface IChatroomsService
    {
        Task<Chatroom> GetOrCreateDirectChatRoom(ChatRoomRequestDTO request);
        Task<Chatroom> CreateGroupChatRoom(CreateGroupChatRoomDTO request);
        Task<ChatroomDTO> GetChatroomById(int chatroomId);
        Task<List<UserChatroomsViewDTO>> GetUserChatrooms(int userId, string roomType);
        Task<bool> UpdateGroupChatroom(int chatroomId, UpdateGroupChatDTO request);
        Task<string> AddUserToChatroom(int chatroomId, int userId);
        Task<string> RemoveUserFromChatroom(int chatroomId, int userId);
        Task<string> MakeUserAdmin(int chatroomId, int userId);
        Task<string> LeaveGroupChatroom(int chatroomId);
    }
}
