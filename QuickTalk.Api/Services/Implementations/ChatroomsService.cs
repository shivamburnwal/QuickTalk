using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;
using QuickTalk.Api.Data.Interfaces;
using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;
using QuickTalk.Api.Services.Implementations;
using QuickTalk.Api.Services.Interfaces;
using QuickTalk.Api.Services.Others;

namespace QuickTalk.Api.Services
{
    public class ChatroomsService : IChatroomsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AuthorizationService _authorizationService;

        public ChatroomsService(IUnitOfWork unitOfWork, AuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }

        public async Task<Chatroom> GetOrCreateDirectChatRoom(ChatRoomRequestDTO request)
        {
            var sender = await _unitOfWork.UsersRepository.GetByIdAsync(request.SenderID);
            var recipient = await _unitOfWork.UsersRepository.GetByIdAsync(request.RecipientID);
            if (sender == null || recipient == null)
                throw new Exception("One or both users not found");

            var chatroom = await _unitOfWork.ChatroomsRepository.GetDirectChatroomAsync(request.SenderID, request.RecipientID);
            if (chatroom != null)
                return chatroom;

            var newChatroom = new Chatroom
            {
                Name = "Room",
                RoomType = ChatroomType.Direct,
                UserChatrooms = new List<UserChatroom>
                {
                    new UserChatroom { UserID = request.SenderID },
                    new UserChatroom { UserID = request.RecipientID }
                }
            };

            await _unitOfWork.ChatroomsRepository.AddAsync(newChatroom);
            await _unitOfWork.SaveChangesAsync();
            return newChatroom;
        }

        public async Task<Chatroom> CreateGroupChatRoom(CreateGroupChatRoomDTO request)
        {
            var sender = await _unitOfWork.UsersRepository.GetByIdAsync(request.SenderID);
            if (sender == null)
                throw new Exception("User not found!");

            var chatroom = new Chatroom
            {
                Name = request.Name,
                Description = request.Description,
                RoomType = ChatroomType.Group,
                UserChatrooms = request.UserIDs.Select(id => new UserChatroom
                {
                    UserID = id,
                    Role = id == request.SenderID ? ChatroomRole.Admin : ChatroomRole.Member
                }).ToList()
            };

            await _unitOfWork.ChatroomsRepository.AddAsync(chatroom);
            await _unitOfWork.SaveChangesAsync();
            return chatroom;
        }

        public async Task<ChatroomDTO> GetChatroomById(int chatroomId)
        {
            var chatroomData = await _unitOfWork.ChatroomsRepository.GetChatroomDataByIdAsync(chatroomId);
            if (chatroomData == null)
                throw new Exception("Chatroom not found!");

            // Validate if user is in the chatroom
            var accessResult = await _authorizationService.ValidateChatroomAccess(chatroomId);
            if (accessResult != null)
                throw new Exception("User is not a part of this chatroom.");

            var response = new ChatroomDTO
            {
                ChatroomID = chatroomData.ChatroomID,
                Name = chatroomData.Name,
                RoomType = chatroomData.RoomType.ToString(),
                Users = chatroomData.UserChatrooms.Select(uc => new UserDTO
                {
                    UserID = uc.User.UserID,
                    Username = uc.User.Username
                }).ToList(),
                Messages = chatroomData.Messages
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDTO
                {
                    MessageID = m.MessageID,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    Sender = (m.User == null) ? null : new UserDTO
                    {
                        UserID = m.User.UserID,
                        Username = m.User.Username
                    }
                }).ToList()
            };

            return response;
        }

        public async Task<List<UserChatroomsViewDTO>> GetUserChatrooms(int userId, string roomType)
        {
            // Convert the roomType string to enum
            if (!Enum.TryParse(roomType, true, out ChatroomType parsedRoomType))
                throw new ArgumentException("Invalid room type specified.");

            var chatrooms = await _unitOfWork.ChatroomsRepository.GetUserChatroomsAsync(userId, parsedRoomType);
            if (chatrooms == null || !chatrooms.Any())
                return new List<UserChatroomsViewDTO>();

            var response = chatrooms.Select(c => new UserChatroomsViewDTO
            {
                ChatroomID = c.ChatroomID,
                Name = c.Name,
            }).ToList();

            return response;
        }

        public async Task<bool> UpdateGroupChatroom(int chatroomId, UpdateGroupChatDTO request)
        {
            // Validate if user is in the chatroom
            var accessResult = await _authorizationService.ValidateChatroomAccess(chatroomId);
            if (accessResult != null)
                throw new Exception("User is not a part of this chatroom.");

            // Is User Admin?
            var isRequesterAdmin = await _authorizationService.ValidateUserAdmin(chatroomId);
            if (!isRequesterAdmin)
                throw new Exception("Only admins can update group details.");

            var chatroom = await _unitOfWork.ChatroomsRepository.GetByIdAsync(chatroomId);
            if (chatroom == null || chatroom.RoomType != ChatroomType.Group)
                return false;

            chatroom.Name = request.GroupName ?? chatroom.Name;
            chatroom.Description = request.Description ?? chatroom.Description;
            chatroom.LastModified = DateTime.UtcNow;

            _unitOfWork.ChatroomsRepository.Update(chatroom);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<string> AddUserToChatroom(int chatroomId, int userId)
        {
            var isChatroomGroup = await _unitOfWork.ChatroomsRepository.IsChatroomTypeGroup(chatroomId);
            if (!isChatroomGroup)
                throw new Exception("Chatroom is not valid.");

            // Is User Admin?
            var isRequesterAdmin = await _authorizationService.ValidateUserAdmin(chatroomId);
            if (!isRequesterAdmin)
                throw new Exception("Only admins can add users.");

            // Check if the user to be added exists
            var newUser = await _unitOfWork.UsersRepository.GetByIdAsync(userId);
            if (newUser == null)
                throw new Exception("User not found.");

            // Check if the user is already a member of the chatroom
            var userChatroom = await _unitOfWork.UserChatroomsRepository.IsUserInChatroomAsync(chatroomId, userId);
            if (userChatroom != null)
                throw new Exception("User is already in the chatroom.");

            // Add the new user to the chatroom
            await _unitOfWork.UserChatroomsRepository.AddUserToChatroomAsync(chatroomId, userId);

            // Update chatroom last modified
            var chatroom = await _unitOfWork.ChatroomsRepository.GetByIdAsync(chatroomId);
            chatroom.LastModified = DateTime.UtcNow;
            _unitOfWork.ChatroomsRepository.Update(chatroom);

            await _unitOfWork.SaveChangesAsync();
            return "User added to chatroom";
        }

        public async Task<string> RemoveUserFromChatroom(int chatroomId, int userId)
        {
            var isChatroomGroup = await _unitOfWork.ChatroomsRepository.IsChatroomTypeGroup(chatroomId);
            if (!isChatroomGroup)
                throw new Exception("Chatroom is not valid.");

            // Is User Admin?
            var isRequesterAdmin = await _authorizationService.ValidateUserAdmin(chatroomId);
            if (!isRequesterAdmin)
                throw new Exception("Only admins can remove users.");

            // Check if the user is a member of the chatroom
            var userChatroom = await _unitOfWork.UserChatroomsRepository.IsUserInChatroomAsync(chatroomId, userId);
            if (userChatroom == null)
                throw new Exception("User is not in the chatroom.");

            // Remove the user from the chatroom
            _unitOfWork.UserChatroomsRepository.Remove(userChatroom);

            // Update chatroom last modified
            var chatroom = await _unitOfWork.ChatroomsRepository.GetByIdAsync(chatroomId);
            chatroom.LastModified = DateTime.UtcNow;
            _unitOfWork.ChatroomsRepository.Update(chatroom);

            await _unitOfWork.SaveChangesAsync();
            return "User removed from chatroom.";
        }

        public async Task<string> MakeUserAdmin(int chatroomId, int userId)
        {
            var isChatroomGroup = await _unitOfWork.ChatroomsRepository.IsChatroomTypeGroup(chatroomId);
            if (!isChatroomGroup)
                throw new Exception("Chatroom is not valid.");

            // Is User Admin?
            var isRequesterAdmin = await _authorizationService.ValidateUserAdmin(chatroomId);
            if (!isRequesterAdmin)
                throw new Exception("Only admins can update group details.");

            // Check if the user is a member of the chatroom
            var userChatroom = await _unitOfWork.UserChatroomsRepository.IsUserInChatroomAsync(chatroomId, userId);
            if (userChatroom == null)
                throw new Exception("User is not in the chatroom.");

            // Check if the user is already an admin
            if (userChatroom.Role == ChatroomRole.Admin)
                throw new Exception("User is already an admin.");

            // Update the user's role to Admin
            userChatroom.Role = ChatroomRole.Admin;
            _unitOfWork.UserChatroomsRepository.Update(userChatroom);

            // Update chatroom last modified
            var chatroom = await _unitOfWork.ChatroomsRepository.GetByIdAsync(chatroomId);
            chatroom.LastModified = DateTime.UtcNow;
            _unitOfWork.ChatroomsRepository.Update(chatroom);

            await _unitOfWork.SaveChangesAsync();
            return "User promoted to admin.";
        }

        public async Task<string> LeaveGroupChatroom(int chatroomId)
        {
            var isChatroomGroup = await _unitOfWork.ChatroomsRepository.IsChatroomTypeGroup(chatroomId);
            if (!isChatroomGroup)
                throw new Exception("Chatroom is not valid.");

            var userId = _authorizationService.GetAuthenticatedUserId();
            if (userId == null)
                throw new Exception("User must be logged in.");

            // Check if the user is a member of the chatroom
            var userChatroom = await _unitOfWork.UserChatroomsRepository.IsUserInChatroomAsync(chatroomId, userId.Value);
            if (userChatroom == null)
                throw new Exception("User is not in the chatroom.");

            var canUserLeave = await _unitOfWork.UserChatroomsRepository.CanUserLeaveChatroom(chatroomId, userId.Value);
            if (!canUserLeave)
                throw new Exception("You cannot leave the group as the last admin. Please assign a new admin before leaving.");

            // Remove user chatroom relationship
            _unitOfWork.UserChatroomsRepository.Remove(userChatroom);

            // Update chatroom last modified
            var chatroom = await _unitOfWork.ChatroomsRepository.GetByIdAsync(chatroomId);
            chatroom.LastModified = DateTime.UtcNow;
            _unitOfWork.ChatroomsRepository.Update(chatroom);

            await _unitOfWork.SaveChangesAsync();
            return "You have left the chatroom.";
        }
    }
}
