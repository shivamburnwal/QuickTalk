namespace QuickTalk.Api.DTOs
{
    public class ChatRoomRequestDTO
    {
        public int SenderID { get; set; }
        public int RecipientID { get; set;}
    }

    public class ChatroomDTO
    {
        public int ChatroomID { get; set; }
        public string Name { get; set; }
        public string RoomType { get; set; }
        public List<UserDTO> Users { get; set; }
        public List<MessageDTO> Messages { get; set; }
    }

    public class CreateGroupChatRoomDTO
    {
        public int SenderID { get; set; }
        public string Name { get; set;}
        public string Description { get; set; }
        public List<int> UserIDs { get; set; }
    }

    public class UpdateGroupChatDTO
    {
        public string? GroupName { get; set; }
        public string? Description { get; set; }
    }

    public class UserChatroomsViewDTO
    {
        public int ChatroomID { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastModified { get; set; }
        public string RoomType { get; set; }
    }
}