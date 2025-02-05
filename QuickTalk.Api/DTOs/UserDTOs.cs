namespace QuickTalk.Api.DTOs
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string? DisplayName { get; set; }
    }

    public class UserProfileDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class UserUpdateDTO
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class UserChatroomDTO
    {
        public int ChatroomID { get; set; }
        public string Name { get; set; }
        public string RoomType { get; set; }
        public bool IsPrivate { get; set; }
    }
}
