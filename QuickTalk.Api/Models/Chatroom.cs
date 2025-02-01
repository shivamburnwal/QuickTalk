namespace QuickTalk.Api.Models
{
    public enum ChatroomType
    {
        Direct, //One-to-one conversations
        Group
    }

    public class Chatroom
    {
        public int ChatroomID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ChatroomType RoomType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPrivate { get; set; }

        public ICollection<UserChatroom> UserChatrooms { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
