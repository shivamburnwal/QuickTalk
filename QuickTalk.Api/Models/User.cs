namespace QuickTalk.Api.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } //Admin or not?
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }

        public RefreshToken RefreshToken { get; set; }
        public ICollection<UserChatroom> UserChatrooms { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<MessageReaction> MessageReactions { get; set; }

        //public ICollection<Conversation> UserConversations { get; set; }
    }
}
