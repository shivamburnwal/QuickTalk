using QuickTalk.Api.DTOs;

namespace QuickTalk.Api.Services.Interfaces
{
    public interface IMessageService
    {
        Task<int> SendMessageAsync(SendMessageRequest request);
        Task<string> DeleteMessageAsync(int messageId);
    }
}
