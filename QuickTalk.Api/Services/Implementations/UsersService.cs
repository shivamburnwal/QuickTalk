using QuickTalk.Api.Data.Interfaces;
using QuickTalk.Api.DTOs;
using QuickTalk.Api.Services.Interfaces;
using System;

namespace QuickTalk.Api.Services.Implementations
{
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserDTO>> SearchUsersAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Search query cannot be empty.");

            var users = await _unitOfWork.UsersRepository.SearchUsersByNameAsync(name);

            if (!users.Any())
                return Enumerable.Empty<UserDTO>();

            return users.Select(u => new UserDTO
            {
                UserID = u.UserID,
                Username = u.Username,
                DisplayName = u.DisplayName
            });
        }

        public async Task<UserProfileDTO?> GetUserProfileAsync(int userId)
        {
            var user = await _unitOfWork.UsersRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            return new UserProfileDTO
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl
            };
        }

        public async Task<bool> UpdateUserProfileAsync(int userId, UserUpdateDTO userUpdateDTO)
        {
            var user = await _unitOfWork.UsersRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.Username = userUpdateDTO.Username ?? user.Username;
            user.Email = userUpdateDTO.Email ?? user.Email;
            user.AvatarUrl = userUpdateDTO.AvatarUrl ?? user.AvatarUrl;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserChatroomDTO>?> GetUserChatroomsAsync(int userId)
        {
            var userChatrooms = await _unitOfWork.UsersRepository.GetUserWithChatroomsAsync(userId);
            if (userChatrooms == null)
                return null;

            if (!userChatrooms.UserChatrooms.Any())
                return Enumerable.Empty<UserChatroomDTO>();

            return userChatrooms.UserChatrooms.Select(uc => new UserChatroomDTO
            {
                ChatroomID = uc.Chatroom.ChatroomID,
                Name = uc.Chatroom.Name,
                RoomType = uc.Chatroom.RoomType.ToString(),
                IsPrivate = uc.Chatroom.IsPrivate
            });
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _unitOfWork.UsersRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            _unitOfWork.UsersRepository.Remove(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
