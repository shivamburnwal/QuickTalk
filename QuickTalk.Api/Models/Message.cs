namespace QuickTalk.Api.Models
{
    public class Message
    {
        public int MessageID { get; set; }
        public int ChatroomID { get; set; }
        public int? UserID { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        //public int? ConversationID { get; set; } // Nullable for chatroom messages

        public Chatroom Chatroom { get; set; }
        public User? User { get; set; }
        public ICollection<MessageReaction> MessageReactions { get; set; }

        //public Conversation Conversation { get; set; }
    }
}