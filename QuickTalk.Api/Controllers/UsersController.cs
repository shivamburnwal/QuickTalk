using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickTalk.Api.DTOs;
using QuickTalk.Api.Services.Interfaces;
using System.Net;

namespace QuickTalk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Search query cannot be empty");

            try
            {
                var users = await _usersService.SearchUsersAsync(name);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            try
            {
                var user = await _usersService.GetUserProfileAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserProfile(int userId, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            try
            {
                var success = await _usersService.UpdateUserProfileAsync(userId, userUpdateDTO);
                if (!success)
                    return NotFound("User not found");

                return Ok("Profile updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{userId}/chatrooms")]
        public async Task<IActionResult> GetUserChatrooms(int userId)
        {
            try
            {
                var chatrooms = await _usersService.GetUserChatroomsAsync(userId);
                if (chatrooms == null)
                    return NotFound("User not found");

                return Ok(chatrooms);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var success = await _usersService.DeleteUserAsync(userId);
                if (!success)
                    return NotFound("User not found");

                return Ok("Account deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}
