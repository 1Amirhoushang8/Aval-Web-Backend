using System.Globalization;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.Common.Exceptions;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Domain.Entities;
using AvalWebBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
        ValidateDto(dto);

        var tx = MapToEntity(dto);

        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (TryParsePersianDate(dto.TransactionDate, out var txDate))
                await _cacheRepo.ArchiveAndResetIfNewPeriodAsync(txDate);

            _context.Transactions.Add(tx);
            await _context.SaveChangesAsync();

            UpdateCaches(tx);
            await _context.SaveChangesAsync();

            await dbTransaction.CommitAsync();
            return MapToDto(tx);
        }
        catch (Exception ex) when (ex is not BusinessRuleException)
        {
            _logger.LogError(ex, "AddAsync failed for transaction {Id}", tx.Id);
            await dbTransaction.RollbackAsync();
            throw new BusinessRuleException("خطا در ثبت تراکنش. لطفاً دوباره تلاش کنید.");
        }
    }

    public async Task<List<TransactionDto>> AddBatchAsync(IEnumerable<CreateTransactionDto> dtos)
    {
        var dtoList = dtos.ToList();
        foreach (var dto in dtoList)
            ValidateDto(dto);

        var results = new List<TransactionDto>();

        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var firstDto = dtoList.FirstOrDefault();
            if (firstDto != null && TryParsePersianDate(firstDto.TransactionDate, out var firstTxDate))
                await _cacheRepo.ArchiveAndResetIfNewPeriodAsync(firstTxDate);

            foreach (var dto in dtoList)
            {
                if (!string.IsNullOrEmpty(dto.IdempotencyKey))
                {
                    var exists = await _context.Transactions
                        .AnyAsync(t => t.IdempotencyKey == dto.IdempotencyKey);
                    if (exists)
                    {
                        _logger.LogInformation("Idempotency key '{Key}' already processed, skipping.", dto.IdempotencyKey);
                        continue;
                    }
                }

                var tx = MapToEntity(dto);
                _context.Transactions.Add(tx);
                results.Add(MapToDto(tx));

                UpdateCaches(tx);
            }

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();
            return results;
        }
        catch (Exception ex) when (ex is not BusinessRuleException)
        {
            _logger.LogError(ex, "AddBatchAsync batch failed");
            await dbTransaction.RollbackAsync();
            throw new BusinessRuleException("خطا در ثبت دسته‌ای تراکنش‌ها. لطفاً دوباره تلاش کنید.");
        }
    }

    // ---------- Validation ----------
    private static void ValidateDto(CreateTransactionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Type))
            throw new BusinessRuleException("نوع تراکنش الزامی است.");

        if (dto.Type != "request" && dto.Type != "payment")
            throw new BusinessRuleException("نوع تراکنش باید 'request' یا 'payment' باشد.");

        if (dto.Amount <= 0)
            throw new BusinessRuleException("مبلغ تراکنش باید بیشتر از صفر باشد.");

        if (string.IsNullOrWhiteSpace(dto.TransactionDate))
            throw new BusinessRuleException("تاریخ تراکنش الزامی است.");

        if (!TryParsePersianDate(dto.TransactionDate, out _))
            throw new BusinessRuleException("فرمت تاریخ باید yyyy-MM-dd باشد و یک تاریخ معتبر شمسی وارد کنید.");
    }

    // ---------- Cache update ----------
    private void UpdateCaches(Transaction tx)
    {
        if (!TryParsePersianDate(tx.TransactionDate, out var txDate))
        {
            _logger.LogWarning("Invalid transaction date skipped for caching: {Date}", tx.TransactionDate);
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
        Type = tx.Type,
        Amount = tx.Amount,
        TransactionDate = tx.TransactionDate,
        Source = tx.Source,
        CreatedAt = tx.CreatedAt
    };

    // ---------- Date helpers ----------
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