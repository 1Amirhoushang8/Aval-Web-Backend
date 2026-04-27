using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Common.Interfaces;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(string id);
    Task AddAsync(Transaction transaction);
    Task AddBatchAsync(IEnumerable<Transaction> transactions);
    Task SaveChangesAsync();
}