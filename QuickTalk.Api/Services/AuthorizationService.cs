using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace QuickTalk.Api.Services
{
    public class AuthorizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetAuthenticatedUserId()
        {
            // Get the authenticated user ID from the JWT
            return _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        // Validate if the user is authenticated and matches the sender's ID
        public IActionResult? ValidateSender(int senderUserId)
        {
            var userId = GetAuthenticatedUserId();

            if (userId == null)
                return new UnauthorizedObjectResult("You must be logged in to send messages.");

            if (senderUserId.ToString() != userId)
                return new UnauthorizedObjectResult("You are not authorized as this user");

            // Valid user
            return null;
        }
    }
}
