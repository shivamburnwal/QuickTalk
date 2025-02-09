using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;

namespace QuickTalk.Api.Repositories.Interfaces
{
    public interface IUsersRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> IsUsernameTakenAsync(string username);
        Task<bool> IsEmailTakenAsync(string email);
        Task<IEnumerable<User>> SearchUsersByNameAsync(string name);
        Task<User?> GetUserWithChatroomsAsync(int userId);
    }
}
