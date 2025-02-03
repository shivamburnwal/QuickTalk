namespace QuickTalk.Api.Models
{
    public class UserChatroom
    {
        public int UserID { get; set; }
        public int ChatroomID { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public Chatroom Chatroom { get; set; }
    }
}
