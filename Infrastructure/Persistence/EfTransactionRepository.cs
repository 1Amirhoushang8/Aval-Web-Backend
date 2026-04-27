using Microsoft.EntityFrameworkCore;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Infrastructure.Persistence;

public class EfTransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;
    public EfTransactionRepository(AppDbContext context) => _context = context;

    public async Task<List<Transaction>> GetAllAsync() =>
        await _context.Transactions.ToListAsync();

    public async Task<Transaction?> GetByIdAsync(string id) =>
        await _context.Transactions.FindAsync(id);

    public async Task AddAsync(Transaction transaction) =>
        await _context.Transactions.AddAsync(transaction);

    public async Task AddBatchAsync(IEnumerable<Transaction> transactions) =>
        await _context.Transactions.AddRangeAsync(transactions);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}