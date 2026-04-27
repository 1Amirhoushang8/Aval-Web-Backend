using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Common.Interfaces;

public interface IStatsCacheRepository
{
    Task<List<DailyStatsCache>> GetDailyStatsAsync();
    Task<List<WeeklyStatsCache>> GetWeeklyStatsAsync();
    Task<List<MonthlyStatsCache>> GetMonthlyStatsAsync();
}