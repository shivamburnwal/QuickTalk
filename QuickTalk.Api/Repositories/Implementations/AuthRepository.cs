using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;
using QuickTalk.Api.Models;
using QuickTalk.Api.Repositories.Interfaces;

namespace QuickTalk.Api.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ChatDbContext _context;

        public AuthRepository(ChatDbContext context)
        {
            _context = context;
        }
    }
}
