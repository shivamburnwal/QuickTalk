using Microsoft.EntityFrameworkCore;
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

        //public DbSet<Conversation> Conversations { get; set; }


        // Relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-many: User -> Message
            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            #region Conversation RelationShips
            /*
            // One-to-many: User -> Conversation (User1)
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User1)
                .WithMany(u => u.UserConversations)
                .HasForeignKey(c => c.User1ID)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many: User -> Conversation (User2)
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User2)
                .WithMany(u => u.UserConversations)
                .HasForeignKey(c => c.User2ID)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many: Conversation -> Message
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationID)
                .OnDelete(DeleteBehavior.Cascade);
            */
            #endregion

            // One-to-many: Chatroom -> Message
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chatroom)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatroomID)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-many: User -> Chatroom (via UserChatroom)
            modelBuilder.Entity<UserChatroom>()
                .HasKey(uc => new { uc.UserID, uc.ChatroomID });

            modelBuilder.Entity<UserChatroom>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserChatrooms)
                .HasForeignKey(uc => uc.UserID);

            modelBuilder.Entity<UserChatroom>()
                .HasOne(uc => uc.Chatroom)
                .WithMany(c => c.UserChatrooms)
                .HasForeignKey(uc => uc.ChatroomID);

            // One-to-many: Message -> MessageReaction
            modelBuilder.Entity<MessageReaction>()
                .HasOne(mr => mr.Message)
                .WithMany(m => m.MessageReactions)
                .HasForeignKey(mr => mr.MessageID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageReaction>()
                .HasOne(mr => mr.User)
                .WithMany(u => u.MessageReactions)
                .HasForeignKey(mr => mr.UserID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
