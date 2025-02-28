namespace QuickTalk.Api.Models
{
    public class RefreshToken
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }

        public User User { get; set; } // One to one relationship btw user and refreshToken
    }
}
