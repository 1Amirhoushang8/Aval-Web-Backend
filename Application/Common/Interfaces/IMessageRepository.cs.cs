using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Common.Interfaces;

public interface IMessageRepository
{
    Task<List<Message>> GetByTicketIdAsync(string ticketId);
    Task<Message?> GetByIdAsync(string messageId);
    Task AddAsync(Message message);
    Task UpdateAsync(Message message);
    Task SaveChangesAsync();
}