using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;
using QuickTalk.Api.Models;
using System.Security.Claims;

namespace QuickTalk.Api.Services.Others
{
    public class AuthorizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ChatDbContext _context;

        public AuthorizationService(ChatDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetAuthenticatedUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        // Validate if the user is part of the chatroom
        public async Task<IActionResult?> ValidateChatroomAccess(int chatroomId)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null)
                return new UnauthorizedObjectResult("You must be logged in.");

            var isUserPartOfChatroom = await _context.UserChatrooms
                                        .AnyAsync(uc => uc.ChatroomID == chatroomId && uc.UserID == userId);

            if (!isUserPartOfChatroom)
                return new UnauthorizedObjectResult("User is not a part of this chatroom.");

            return null; // User is authorized
        }

        public async Task<bool> ValidateUserAdmin(int chatroomId)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null)
                return false;

            return await _context.UserChatrooms
                .AnyAsync(uc => uc.UserID == userId && uc.ChatroomID == chatroomId && uc.Role == ChatroomRole.Admin);
        }
    }
}
