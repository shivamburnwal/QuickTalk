using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickTalk.Api.DTOs;
using QuickTalk.Api.Services.Interfaces;

namespace QuickTalk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatroomsController : ControllerBase
    {
        private readonly IChatroomsService _chatroomsService;

        public ChatroomsController(IChatroomsService chatroomsService)
        {
            _chatroomsService = chatroomsService;
        }

        // Get or Create Direct Chatroom
        [HttpPost("direct")]
        public async Task<IActionResult> GetOrCreateDirectChatRoom([FromBody] ChatRoomRequestDTO request)
        {
            try
            {
                var chatroom = await _chatroomsService.GetOrCreateDirectChatRoom(request);
                return Ok(new { ChatroomID = chatroom.ChatroomID });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Create a Group Chatroom
        [HttpPost("createGroup")]
        public async Task<IActionResult> CreateGroupChatRoom([FromBody] CreateGroupChatRoomDTO request)
        {
            try
            {
                var chatroom = await _chatroomsService.CreateGroupChatRoom(request);
                return Ok(new { chatroom.ChatroomID });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get Chatroom by ID
        [HttpGet("{chatroomId}")]
        public async Task<IActionResult> GetChatroom(int chatroomId)
        {
            try
            {
                var chatroom = await _chatroomsService.GetChatroomById(chatroomId);
                return Ok(chatroom);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get Direct or Group Chatrooms for User
        [HttpGet("user/{userId}/{roomType}")]
        public async Task<IActionResult> GetUserChatrooms(int userId, string roomType)
        {
            try
            {
                var chatrooms = await _chatroomsService.GetUserChatrooms(userId, roomType);
                return Ok(chatrooms);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Update Group Chatroom
        [HttpPut("{chatroomId}")]
        public async Task<IActionResult> UpdateGroupChatroom(int chatroomId, [FromBody] UpdateGroupChatDTO request)
        {
            try
            {
                var success = await _chatroomsService.UpdateGroupChatroom(chatroomId, request);
                if (success)
                    return Ok("Group chatroom updated successfully.");
                return BadRequest("Failed to update group chatroom.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Add User to Group Chatroom
        [HttpPost("{chatroomId}/addUser/{userId}")]
        public async Task<IActionResult> AddUserToChatroom(int chatroomId, int userId)
        {
            try
            {
                var response = await _chatroomsService.AddUserToChatroom(chatroomId, userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Remove User from Group Chatroom
        [HttpDelete("{chatroomId}/removeUser/{userId}")]
        public async Task<IActionResult> RemoveUserFromChatroom(int chatroomId, int userId)
        {
            try
            {
                var response = await _chatroomsService.RemoveUserFromChatroom(chatroomId, userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Make User Admin
        [HttpPost("{chatroomId}/makeAdmin/{userId}")]
        public async Task<IActionResult> MakeUserAdmin(int chatroomId, int userId)
        {
            try
            {
                var response = await _chatroomsService.MakeUserAdmin(chatroomId, userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Leave Group Chatroom
        [HttpPost("{chatroomId}/leave")]
        public async Task<IActionResult> LeaveGroupChatroom(int chatroomId)
        {
            try
            {
                var response = await _chatroomsService.LeaveGroupChatroom(chatroomId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}