using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;

    public TransactionService(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<TransactionDto> AddAsync(CreateTransactionDto dto)
    {
        
        if (!string.IsNullOrEmpty(dto.IdempotencyKey))
        {
            var all = await _repository.GetAllAsync();
            var existing = all.FirstOrDefault(t => t.IdempotencyKey == dto.IdempotencyKey);
            if (existing != null)
            {
                return MapToDto(existing);
            }
        }

        var tx = new Transaction
        {
            Id = Guid.NewGuid().ToString(),
            UserId = dto.UserId ?? string.Empty,
            TicketId = dto.TicketId ?? string.Empty,
            Type = dto.Type,
            Amount = dto.Amount,
            TransactionDate = dto.TransactionDate,
            Source = dto.Source,
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            IdempotencyKey = dto.IdempotencyKey
        };

        await _repository.AddAsync(tx);
        await _repository.SaveChangesAsync();

        return MapToDto(tx);
    }

    public async Task<List<TransactionDto>> AddBatchAsync(IEnumerable<CreateTransactionDto> dtos)
    {
        var allTransactions = await _repository.GetAllAsync();
        var results = new List<TransactionDto>();

        foreach (var dto in dtos)
        {
            // Idempotency check
            if (!string.IsNullOrEmpty(dto.IdempotencyKey) &&
                allTransactions.Any(t => t.IdempotencyKey == dto.IdempotencyKey))
            {
               
                continue;
            }

            var tx = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                UserId = dto.UserId ?? string.Empty,
                TicketId = dto.TicketId ?? string.Empty,
                Type = dto.Type,
                Amount = dto.Amount,
                TransactionDate = dto.TransactionDate,
                Source = dto.Source,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                IdempotencyKey = dto.IdempotencyKey
            };

            await _repository.AddAsync(tx);
            results.Add(MapToDto(tx));

            
            allTransactions.Add(tx);
        }

        await _repository.SaveChangesAsync();
        return results;
    }

    private static TransactionDto MapToDto(Transaction tx) => new()
    {
        Id = tx.Id,
        UserId = tx.UserId,
        TicketId = tx.TicketId,
        Type = tx.Type,
        Amount = tx.Amount,
        TransactionDate = tx.TransactionDate,
        Source = tx.Source,
        CreatedAt = tx.CreatedAt
    };
}