using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Extensions;
using QuickTalk.Api.Models;

namespace QuickTalk.Api.Data
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options)
        {
        }

        // Create Models
        public DbSet<User> Users { get; set; }
        public DbSet<Chatroom> Chatrooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserChatroom> UserChatrooms { get; set; }
        public DbSet<MessageReaction> MessageReactions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        //public DbSet<Conversation> Conversations { get; set; }


        // Relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define relationships
            modelBuilder.ConfigureRelationships();

            // Seed initial data
            modelBuilder.Seed();
        }
    }
}
