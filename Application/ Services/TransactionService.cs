using System.Globalization;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Domain.Entities;
using AvalWebBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AvalWebBackend.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;
    private readonly IStatsCacheRepository _cacheRepo;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        AppDbContext context,
        IStatsCacheRepository cacheRepo,
        ILogger<TransactionService> logger)
    {
        _context = context;
        _cacheRepo = cacheRepo;
        _logger = logger;
    }

    public async Task<TransactionDto> AddAsync(CreateTransactionDto dto)
    {
        var tx = MapToEntity(dto);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Transactions.Add(tx);
            await _context.SaveChangesAsync();

            UpdateCaches(tx);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return MapToDto(tx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add transaction");
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<TransactionDto>> AddBatchAsync(IEnumerable<CreateTransactionDto> dtos)
    {
        var results = new List<TransactionDto>();

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var dto in dtos)
            {
                // Idempotency check
                if (!string.IsNullOrEmpty(dto.IdempotencyKey))
                {
                    var exists = await _context.Transactions
                        .AnyAsync(t => t.IdempotencyKey == dto.IdempotencyKey);
                    if (exists) continue;
                }

                var tx = MapToEntity(dto);
                _context.Transactions.Add(tx);
                results.Add(MapToDto(tx));

                UpdateCaches(tx);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Batch transaction failed");
            await transaction.RollbackAsync();
            throw;
        }

        return results;
    }

    // ---------- Cache update helper ----------
    private void UpdateCaches(Transaction tx)
    {
        if (!TryParsePersianDate(tx.TransactionDate, out var txDate))
        {
            _logger.LogWarning("Invalid transaction date: {Date}", tx.TransactionDate);
            return;
        }

        _cacheRepo.UpsertDailyCache(tx, txDate);
        _cacheRepo.UpsertWeeklyCache(tx, txDate);
        _cacheRepo.UpsertMonthlyCache(tx, txDate);
    }

    // ---------- Mapping ----------
    private static Transaction MapToEntity(CreateTransactionDto dto) => new()
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

    private static bool TryParsePersianDate(string persianDate, out DateTime result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(persianDate)) return false;
        var parts = persianDate.Split('-');
        if (parts.Length != 3) return false;
        if (!int.TryParse(parts[0], out int year) ||
            !int.TryParse(parts[1], out int month) ||
            !int.TryParse(parts[2], out int day)) return false;
        try
        {
            var pc = new PersianCalendar();
            result = pc.ToDateTime(year, month, day, 0, 0, 0, 0);
            return true;
        }
        catch { return false; }
    }
}