using QuickTalk.Api.Data.Interfaces;
using QuickTalk.Api.Repositories.Interfaces;

namespace QuickTalk.Api.Data.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChatDbContext _context;
        public IUsersRepository UsersRepository { get; }
        public IChatroomsRepository ChatroomsRepository { get; }
        public IMessageRepository MessageRepository { get; }
        public IAuthRepository AuthRepository { get; }
        public IUserChatroomsRepository UserChatroomsRepository { get; }

        public UnitOfWork(ChatDbContext context, IUsersRepository usersRepository, IAuthRepository authRepository, 
            IChatroomsRepository chatroomsRepository, IMessageRepository messageRepository, IUserChatroomsRepository userChatroomsRepository)
        {
            _context = context;
            UsersRepository = usersRepository;
            AuthRepository = authRepository;
            ChatroomsRepository = chatroomsRepository;
            MessageRepository = messageRepository;
            UserChatroomsRepository = userChatroomsRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
