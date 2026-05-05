using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Common.Interfaces;

public interface IServiceRepository
{
    Task<List<Service>> GetAllAsync();
    Task<List<Service>> GetByUserIdAsync(string userId);
    Task<Service?> GetByIdAsync(string id);
    Task AddAsync(Service service);
    Task UpdateAsync(Service service);
    Task DeleteAsync(string id);
    Task SaveChangesAsync();
}