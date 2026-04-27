using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Infrastructure.Persistence;

public class JsonTransactionRepository : ITransactionRepository
{
    private readonly JsonDataService _dataService;

    public JsonTransactionRepository(JsonDataService dataService)
    {
        _dataService = dataService;
    }

    private async Task<List<Transaction>> GetTransactionsAsync()
    {
        var db = await _dataService.ReadAsync();
        return db.Transactions ?? new List<Transaction>();
    }

    public async Task<List<Transaction>> GetAllAsync()
    {
        return await GetTransactionsAsync();
    }

    public async Task<Transaction?> GetByIdAsync(string id)
    {
        var transactions = await GetTransactionsAsync();
        return transactions.FirstOrDefault(t => t.Id == id);
    }

    public async Task AddAsync(Transaction transaction)
    {
        var db = await _dataService.ReadAsync();
        db.Transactions.Add(transaction);
        await _dataService.WriteAsync(db);
    }

    public async Task AddBatchAsync(IEnumerable<Transaction> transactions)
    {
        var db = await _dataService.ReadAsync();
        db.Transactions.AddRange(transactions);
        await _dataService.WriteAsync(db);
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}