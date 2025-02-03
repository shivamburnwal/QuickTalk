namespace QuickTalk.Api.Models
{
    public enum ChatroomRole
    {
        Member,
        Admin
    }

    public class UserChatroom
    {
        public int UserID { get; set; }
        public int ChatroomID { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;
        public ChatroomRole Role { get; set; }  // New field to store user role

        public User User { get; set; }
        public Chatroom Chatroom { get; set; }
    }
}
