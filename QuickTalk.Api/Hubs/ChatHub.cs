using Microsoft.AspNetCore.SignalR;
using QuickTalk.Api.Data.Interfaces;

namespace QuickTalk.Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Send message to a specific chatroom (direct/group)
        public async Task SendMessage(string chatroomId, string senderUsername, string message)
        {
            var user = await _unitOfWork.UsersRepository.GetUserByUsernameAsync(senderUsername);

            // Sender
            var sender = new {
                user.UserID,
                user.Username
            };

            await Clients.Group(chatroomId).SendAsync("ReceiveMessage", chatroomId, sender, message);
        }

        // Join a chatroom (group)
        public async Task JoinChatroom(string chatroomId)
        {
            if (string.IsNullOrEmpty(chatroomId))
            {
                throw new HubException("Chatroom ID cannot be null or empty.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);
        }

        // Leave a chatroom
        public async Task LeaveChatroom(string chatroomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatroomId);
        }
    }
}
