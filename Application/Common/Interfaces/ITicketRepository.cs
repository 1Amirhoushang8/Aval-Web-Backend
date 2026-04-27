using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Common.Interfaces;

public interface ITicketRepository
{
    Task<List<Ticket>> GetAllAsync();
    Task<Ticket?> GetByIdAsync(string id);
    Task<Ticket?> GetByIdWithFileAsync(string id);
    Task AddAsync(Ticket ticket);
    Task<bool> ExistsAsync(string id);
    Task UpdateAsync(Ticket ticket);
    Task DeleteAsync(string id);
    Task SaveChangesAsync();
}