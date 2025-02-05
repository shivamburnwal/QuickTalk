using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;
using QuickTalk.Api.DTOs;
using QuickTalk.Api.Services;

namespace QuickTalk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ChatDbContext _context;
        private readonly AuthorizationService _authService;

        public UsersController(ChatDbContext context, AuthorizationService authSerivce)
        {
            _context = context;
            _authService = authSerivce;
        }

        // Search users
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Search query cannot be empty");

            var users = await _context.Users
                .Where(u => u.Username.Contains(name))
                .Select(u => new UserDTO
                {
                    UserID = u.UserID,
                    Username = u.Username,
                    DisplayName = u.DisplayName
                })
                .ToListAsync();

            return Ok(users);
        }

        // Get user
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(int userId)
        { 
            var user = await _context.Users
                .Where(u => u.UserID == userId)
                .Select(u => new UserProfileDTO
                {
                    UserID = u.UserID,
                    Username = u.Username,
                    Email = u.Email,
                    DisplayName = u.DisplayName,
                    AvatarUrl = u.AvatarUrl
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        // Update user profile
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserProfile(int userId, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            // Update user properties
            user.Username = userUpdateDTO.Username ?? user.Username;
            user.Email = userUpdateDTO.Email ?? user.Email;
            user.AvatarUrl = userUpdateDTO.AvatarUrl ?? user.AvatarUrl;

            await _context.SaveChangesAsync();

            return Ok("Profile updated successfully.");
        }

        // Get chatrooms for user
        [HttpGet("{userId}/chatrooms")]
        public async Task<IActionResult> GetUserChatrooms(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserChatrooms)
                .ThenInclude(uc => uc.Chatroom)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null)
                return NotFound("User not found");

            var chatrooms = user.UserChatrooms.Select(uc => new UserChatroomDTO
            {
                ChatroomID = uc.Chatroom.ChatroomID,
                Name = uc.Chatroom.Name,
                RoomType = uc.Chatroom.RoomType.ToString(),
                IsPrivate = uc.Chatroom.IsPrivate
            });

            return Ok(chatrooms);
        }

        // Delete user
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("Account deleted successfully.");
        }
    }
}
