using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Models;
using QuickTalk.Api.Services.Others;

namespace QuickTalk.Api.Data
{
    public static class SeedData
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Hash the password
            var adminPassword = PasswordService.HashPassword("Admin@123");

            modelBuilder.Entity<User>().HasData(new User
            {
                UserID = 1,
                Username = "admin",
                PasswordHash = adminPassword,
                Email = "admin@quicktalk.com",
                Role = "Admin"
            });
        }
    }
}
