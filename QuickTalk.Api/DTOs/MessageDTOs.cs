namespace QuickTalk.Api.DTOs
{
    public class SendMessageRequest
    {
        public string Content { get; set;}
        public int ChatRoomID { get; set;}
    }

    public class MessageDTO
    {
        public int MessageID { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public UserDTO? Sender { get; set; }
    }
}
