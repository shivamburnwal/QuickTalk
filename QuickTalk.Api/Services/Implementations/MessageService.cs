using QuickTalk.Api.Data.Interfaces;
using QuickTalk.Api.DTOs;
using QuickTalk.Api.Models;
using QuickTalk.Api.Services.Interfaces;
using QuickTalk.Api.Services.Others;

namespace QuickTalk.Api.Services.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AuthorizationService _authorizationService;

        public MessageService(IUnitOfWork unitOfWork, AuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }

        public async Task<MessageDTO> GetMessageByIdAsync(int messageId)
        {
            var message = await _unitOfWork.MessageRepository.GetByIdAsync(messageId);
            if (message == null)
                throw new Exception("Message not found.");

            var user = message.UserID != null
                ? await _unitOfWork.UsersRepository.GetByIdAsync((int)message.UserID)
                : null;

            return new MessageDTO
            {
                MessageID = message.MessageID,
                Content = message.Content,
                SentAt = message.SentAt,
                Sender = user != null ? new UserDTO
                {
                    UserID = user.UserID,
                    Username = user.Username
                } : null
            };
        }

        public async Task<int> SendMessageAsync(SendMessageRequest request)
        {
            var senderId = _authorizationService.GetAuthenticatedUserId();
            if (senderId == null)
                throw new UnauthorizedAccessException("User is not authorised!");

            var chatRoom = await _unitOfWork.ChatroomsRepository.GetByIdAsync(request.ChatRoomID);
            if (chatRoom == null)
                throw new Exception("Chatroom not found");

            var accessResult = await _authorizationService.ValidateChatroomAccess(chatRoom.ChatroomID);
            if (accessResult != null)
                throw new UnauthorizedAccessException("User not authorized to send messages in this chatroom");

            var message = new Message
            {
                ChatroomID = request.ChatRoomID,
                UserID = senderId,
                Content = request.Content,
                SentAt = DateTime.UtcNow
            };

            // Add message.
            await _unitOfWork.MessageRepository.AddAsync(message);

            // Update chatroom last modified.
            chatRoom.LastModified = DateTime.UtcNow;
            _unitOfWork.ChatroomsRepository.Update(chatRoom);

            await _unitOfWork.SaveChangesAsync();
            return message.MessageID;
        }

        public async Task<string> DeleteMessageAsync(int messageId)
        {
            var userId = _authorizationService.GetAuthenticatedUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("User is not authorized!");

            var message = await _unitOfWork.MessageRepository.GetByIdAsync(messageId);
            if (message == null)
                throw new Exception("Message not found");

            if (message.UserID != userId)
                throw new Exception("User not authorized to delete this message");

            // Remove message.
            _unitOfWork.MessageRepository.Remove(message);

            // Update chatroom last modified.
            var chatRoomId = await _unitOfWork.MessageRepository.GetChatroomByMessageIdAsync(messageId);
            var chatRoom = await _unitOfWork.ChatroomsRepository.GetByIdAsync(chatRoomId);
            if (chatRoom == null)
                throw new Exception("Chatroom not found");
            chatRoom.LastModified = DateTime.UtcNow;
            _unitOfWork.ChatroomsRepository.Update(chatRoom);

            await _unitOfWork.SaveChangesAsync();
            return "Message Deleted Succeddfully.";
        }
    }
}
