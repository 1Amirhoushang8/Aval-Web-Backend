using AvalWebBackend.Application.DTOs;

namespace AvalWebBackend.Application.Common.Interfaces;

public interface IMessageService
{
    Task<IEnumerable<MessageDto>> GetMessagesByTicketAsync(string ticketId);
    Task<MessageDto> SendMessageAsync(SendMessageRequest request);
    Task<MessageDto> MarkAsReadAsync(string messageId);
    Task MarkMessagesAsReadAsync(string ticketId, string readerType);
}