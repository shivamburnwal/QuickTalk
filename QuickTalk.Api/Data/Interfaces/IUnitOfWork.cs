using QuickTalk.Api.Repositories.Interfaces;

namespace QuickTalk.Api.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUsersRepository UsersRepository { get; }
        IChatroomsRepository ChatroomsRepository { get; }
        IMessageRepository MessageRepository { get; }
        IAuthRepository AuthRepository { get; }
        IUserChatroomsRepository UserChatroomsRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
