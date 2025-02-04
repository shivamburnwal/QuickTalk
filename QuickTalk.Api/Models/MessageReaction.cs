using System.ComponentModel.DataAnnotations;

namespace QuickTalk.Api.Models
{
    public class MessageReaction
    {
        [Key]
        public int ReactionID { get; set; }
        public int MessageID { get; set; }
        public int? UserID { get; set; }
        public string ReactionType { get; set; } // Like, Love, Angry etc..
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Message Message { get; set; }
        public User? User { get; set; }
    }
}