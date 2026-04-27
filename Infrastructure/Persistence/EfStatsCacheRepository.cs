using Microsoft.EntityFrameworkCore;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Infrastructure.Persistence;

public class EfStatsCacheRepository : IStatsCacheRepository
{
    private readonly AppDbContext _context;

    public EfStatsCacheRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DailyStatsCache>> GetDailyStatsAsync() =>
        await _context.DailyStatsCache.ToListAsync();

    public async Task<List<WeeklyStatsCache>> GetWeeklyStatsAsync() =>
        await _context.WeeklyStatsCache.ToListAsync();

    public async Task<List<MonthlyStatsCache>> GetMonthlyStatsAsync() =>
        await _context.MonthlyStatsCache.ToListAsync();
}