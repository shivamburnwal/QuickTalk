using QuickTalk.Api.DTOs;

namespace QuickTalk.Api.Services.Interfaces
{
    public interface IMessageService
    {
        Task<MessageDTO> GetMessageByIdAsync(int messageId);
        Task<int> SendMessageAsync(SendMessageRequest request);
        Task<string> DeleteMessageAsync(int messageId);
    }
}
