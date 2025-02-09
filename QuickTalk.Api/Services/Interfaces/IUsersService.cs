using QuickTalk.Api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickTalk.Api.Services.Interfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<UserDTO>> SearchUsersAsync(string name);
        Task<UserProfileDTO?> GetUserProfileAsync(int userId);
        Task<bool> UpdateUserProfileAsync(int userId, UserUpdateDTO userUpdateDTO);
        Task<IEnumerable<UserChatroomDTO>?> GetUserChatroomsAsync(int userId);
        Task<bool> DeleteUserAsync(int userId);
    }
}
