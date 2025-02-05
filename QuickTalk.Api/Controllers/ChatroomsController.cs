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
    public class ChatroomsController : ControllerBase
    {
        private readonly ChatDbContext _context;
        private readonly AuthorizationService _authService;

        public ChatroomsController(ChatDbContext context, AuthorizationService authService)
        {
            _context = context;
            _authService = authService;
        }

        // Get or Create Direct Chatroom
        [HttpPost]
        public async Task<IActionResult> GetOrCreateDirectChatRoom([FromBody] ChatRoomRequestDTO request)
        {
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.UserID == request.SenderID);
            var recipient = await _context.Users.FirstOrDefaultAsync(u => u.UserID == request.RecipientID);

            if (sender == null || recipient == null)
                return NotFound("One or both users not found");

            // Check if a chat room already exists
            var chatroom = await _context.Chatrooms
                .Include(cr => cr.UserChatrooms)
                .FirstOrDefaultAsync(cr =>
                    cr.UserChatrooms.Any(uc => uc.UserID == sender.UserID) &&
                    cr.UserChatrooms.Any(uc => uc.UserID == recipient.UserID) &&
                    !String.Equals(cr.RoomType, "Group")
                );

            // If chat room exists, return it.
            if (chatroom != null)
                return Ok(new { ChatroomID = chatroom.ChatroomID });

            // If no chat room exists, create one.
            chatroom = new Chatroom
            {
                Name = "Room",
                RoomType = ChatroomType.Direct,
                UserChatrooms = new List<UserChatroom>
                {
                    new UserChatroom { UserID = sender.UserID },
                    new UserChatroom { UserID = recipient.UserID }
                }
            };

            _context.Chatrooms.Add(chatroom);
            await _context.SaveChangesAsync();

            return Ok(new { ChatroomID = chatroom.ChatroomID });
        }

        // Create a Group Chatroom
        [HttpPost("createGroup")]
        public async Task<IActionResult> CreateGroupChatRoom([FromBody] CreateGroupChatRoomDTO request)
        {
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.UserID == request.SenderID);
            if (sender == null)
                return NotFound("User not found.");

            var chatroom = new Chatroom
            {
                Name = request.Name,
                Description = request.Description,
                RoomType = ChatroomType.Group,
                UserChatrooms = request.UserIDs
                    .Where(id => id != sender.UserID)
                    .Select(id => new UserChatroom 
                    {
                        UserID = id,
                        Role = ChatroomRole.Member
                    })
                    .ToList()
            };

            // Set creator as Admin
            chatroom.UserChatrooms.Add(new UserChatroom
            {
                UserID = sender.UserID,
                Role = ChatroomRole.Admin
            });

            _context.Chatrooms.Add(chatroom);
            await _context.SaveChangesAsync();

            return Ok(new { chatroom.ChatroomID });
        }

        // Get Chatroom
        [HttpGet("{chatroomId}")]
        public async Task<IActionResult> GetChatroom(int chatroomId)
        {
            var chatroom = await _context.Chatrooms
                .Include(c => c.UserChatrooms).ThenInclude(uc => uc.User)
                .Include(c => c.Messages).ThenInclude(m => m.User)
                .FirstOrDefaultAsync(c => c.ChatroomID == chatroomId);

            if (chatroom == null)
                return NotFound("Chatroom not found.");

            // Validate if user is in the chatroom
            var accessResult = await _authService.ValidateChatroomAccess(chatroomId);
            if (accessResult != null)
                return accessResult;

            var response = new ChatroomDTO
            {
                ChatroomID = chatroom.ChatroomID,
                Name = chatroom.Name,
                RoomType = chatroom.RoomType.ToString(),
                Users = chatroom.UserChatrooms.Select(uc => new UserDTO
                { 
                    UserID = uc.User.UserID, 
                    Username = uc.User.Username 
                }).ToList(),
                Messages = chatroom.Messages
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDTO
                {
                    MessageID = m.MessageID,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    Sender = ( m.User == null ) ? null : new UserDTO
                             {
                                UserID = m.User.UserID,
                                Username = m.User.Username
                             }
                             }).ToList()
            };

            return Ok(response);
        }

        // Get Direct Or Group Chatrooms For User
        [HttpGet("{userId}/{roomType}")]
        public async Task<IActionResult> GetUserChatrooms(int userId, string roomType)
        {
            // Convert the roomType string to enum
            if (!Enum.TryParse(roomType, true, out ChatroomType parsedRoomType))
                return BadRequest("Invalid room type specified.");

            // Get chatrooms the user is part of
            var chatrooms = await _context.Chatrooms
                .Include(c => c.UserChatrooms)
                .ThenInclude(uc => uc.User)
                .Include(c => c.Messages)
                .Where(c => c.UserChatrooms.Any(uc => uc.UserID == userId) &&
                            (
                                ( parsedRoomType == ChatroomType.Group ) ?
                                ( c.RoomType == ChatroomType.Group ) :
                                ( c.RoomType == ChatroomType.Direct && c.Messages.Any() )
                            ))
                .ToListAsync();

            if (chatrooms == null || !chatrooms.Any())
                return NotFound("No chatrooms found for this user.");

            var response = chatrooms.Select(c => new UserChatroomsViewDTO
            {
                ChatroomID = c.ChatroomID,
                Name = c.Name,
            }).ToList();

            return Ok(response);
        }

        // Update Group Chatroom
        [HttpPut("{chatroomId}")]
        public async Task<IActionResult> UpdateGroupChatroom(int chatroomId, [FromBody] UpdateGroupChatDTO request)
        {
            // Validate if user is in the chatroom
            var accessResult = await _authService.ValidateChatroomAccess(chatroomId);
            if (accessResult != null)
                return accessResult;

            // Is User Admin?
            var isRequesterAdmin = await _authService.ValidateUserAdmin(chatroomId);
            if (!isRequesterAdmin)
                return new UnauthorizedObjectResult("Only admins can update group details.");

            var chatroom = await _context.Chatrooms.FindAsync(chatroomId);
            if (chatroom == null || chatroom.RoomType != ChatroomType.Group)
                return NotFound("Group chat not found.");

            chatroom.Name = request.GroupName ?? chatroom.Name;
            chatroom.Description = request.Description ?? chatroom.Description;

            await _context.SaveChangesAsync();
            return Ok("Chatroom updated successfully.");
        }

        // Add User to Chatroom
        [HttpPost("{chatroomId}/addUser")]
        public async Task<IActionResult> AddUserToChatroom(int chatroomId, [FromBody] AddUserRequestDTO request)
        {
            // Check if the current user is an admin of the chatroom
            var isRequesterAdmin = await _authService.ValidateUserAdmin(chatroomId);
            if (!isRequesterAdmin)
                return new UnauthorizedObjectResult("Only admins of chatroom add users.");

            // Check if the user to be added exists
            var newUser = await _context.Users.FindAsync(request.UserID);
            if (newUser == null)
                return NotFound("User not found.");

            // Check if the user is already a member of the chatroom
            var isAlreadyMember = await _context.UserChatrooms
                .AnyAsync(uc => uc.ChatroomID == chatroomId && uc.UserID == request.UserID);

            if (isAlreadyMember)
                return BadRequest("User is already in the chatroom.");

            // Add the new user to the chatroom
            _context.UserChatrooms.Add(new UserChatroom
            {
                UserID = request.UserID,
                ChatroomID = chatroomId,
                Role = ChatroomRole.Member  // Default role is member
            });

            await _context.SaveChangesAsync();

            return Ok("User added successfully.");
        }

        // Remove User from Chatroom
        [HttpPost("{chatroomId}/removeUser")]
        public async Task<IActionResult> RemoveUserFromChatroom(int chatroomId, [FromBody] RemoveUserRequestDTO request)
        {
            // Check if the current user is an admin of the chatroom
            var isRequesterAdmin = await _authService.ValidateUserAdmin(chatroomId);
            if (!isRequesterAdmin)
                return new UnauthorizedObjectResult("Only admins of chatroom can remove users.");

            // Find the user in the chatroom
            var userChatroom = await _context.UserChatrooms
                .FirstOrDefaultAsync(uc => uc.ChatroomID == chatroomId && uc.UserID == request.UserID);

            if (userChatroom == null)
                return NotFound("User is not in the chatroom.");

            // Remove the user from the chatroom
            _context.UserChatrooms.Remove(userChatroom);
            await _context.SaveChangesAsync();

            return Ok("User removed successfully.");
        }

        // Make User Admin
        [HttpPost("{chatroomId}/makeAdmin")]
        public async Task<IActionResult> MakeUserAdmin(int chatroomId, [FromBody] MakeAdminRequestDTO request)
        {
            // Check if the current user is an admin of the chatroom
            var isRequesterAdmin = await _authService.ValidateUserAdmin(chatroomId);
            if (!isRequesterAdmin)
                return new UnauthorizedObjectResult("Only admins of chatroom can make other users admins.");

            // Check if the user exists and is a member of the chatroom
            var userChatroom = await _context.UserChatrooms
                .FirstOrDefaultAsync(uc => uc.ChatroomID == chatroomId && uc.UserID == request.UserID);

            if (userChatroom == null)
                return NotFound("User is not a member of the chatroom.");

            // Check if the user is already an admin
            if (userChatroom.Role == ChatroomRole.Admin)
                return BadRequest("User is already an admin.");

            // Update the user's role to Admin
            userChatroom.Role = ChatroomRole.Admin;
            await _context.SaveChangesAsync();

            return Ok("User is now an admin.");
        }

        // Leave Group Chatroom
        [HttpPost("{chatroomId}/leave")]
        public async Task<IActionResult> LeaveGroup(int chatroomId)
        {
            var userId = _authService.GetAuthenticatedUserId();
            if (userId == null)
                return Unauthorized("User must be logged in.");

            // Check if the user is a member of the chatroom
            var userChatroom = await _context.UserChatrooms
                .FirstOrDefaultAsync(uc => uc.ChatroomID == chatroomId && uc.UserID == userId.Value);

            if (userChatroom == null)
                return NotFound("User is not a member of the chatroom.");

            // Check if the user is an admin, if so, restrict leaving if they're the last admin
            var adminCount = await _context.UserChatrooms
                .Where(uc => uc.ChatroomID == chatroomId && uc.Role == ChatroomRole.Admin)
                .CountAsync();

            if (userChatroom.Role == ChatroomRole.Admin && adminCount == 1)
                return BadRequest("You cannot leave the group while you're the only admin. Please assign a new admin before leaving.");

            // Remove the user from the chatroom
            _context.UserChatrooms.Remove(userChatroom);
            await _context.SaveChangesAsync();

            return Ok("You have left the group.");
        }
    }
}