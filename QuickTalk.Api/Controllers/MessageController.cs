using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;
using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;
using QuickTalk.Api.Services;

namespace QuickTalk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly ChatDbContext _context;
        private readonly AuthorizationService _authService;

        public MessageController(ChatDbContext context, AuthorizationService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            // Check if the sender exists
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.SenderUsername);
            if (sender == null)
                return NotFound("Sender not found");

            // Check if the chatroom exists
            var chatRoom = await _context.Chatrooms.FindAsync(request.ChatRoomID);
            if (chatRoom == null)
                return NotFound("Chat room not found");

            // Validate if user is in the chatroom
            var accessResult = await _authService.ValidateChatroomAccess(chatRoom.ChatroomID);
            if (accessResult != null)
                return accessResult;

            var message = new Message {
                ChatroomID = request.ChatRoomID,
                UserID = sender.UserID,
                Content = request.Content,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(new { messageId = message.MessageID });
        }
    }
}