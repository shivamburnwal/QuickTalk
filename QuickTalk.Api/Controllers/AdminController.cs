using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickTalk.Api.Services.Others;
using System.Data;

namespace QuickTalk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        private readonly IWebHostEnvironment _env;

        public AdminController(DatabaseService databaseService, IWebHostEnvironment env)
        {
            _databaseService = databaseService;
            _env = env;
        }

        [HttpPost("truncate-database")]
        [Authorize(Roles = "Admin")]
        public IActionResult TruncateDatabase()
        {
            try
            {
                // Check if the environment is Development
                if (_env.IsDevelopment())
                {
                    _databaseService.DeleteDataFromDatabase();
                    return Ok("Database has been truncated successfully.");
                }
                else
                {
                    return BadRequest("This action is only allowed in the development environment.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while truncating the database: {ex.Message}");
            }
        }
    }
}
