namespace QuickTalk.Api.Models
{
    public class Conversation
    {
        public int ConversationID { get; set; }
        public int User1ID { get; set; }
        public int User2ID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; }
        public User User1 { get; set; }
        public User User2 { get; set; }
    }
}