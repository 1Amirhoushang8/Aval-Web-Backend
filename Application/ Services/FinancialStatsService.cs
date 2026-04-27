using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Services;

public class FinancialStatsService : IFinancialStatsService
{
    private readonly IStatsCacheRepository _cacheRepository;
    private readonly ILogger<FinancialStatsService> _logger;

    public FinancialStatsService(
        IStatsCacheRepository cacheRepository,
        ILogger<FinancialStatsService> logger)
    {
        _cacheRepository = cacheRepository;
        _logger = logger;
    }

    public async Task<FinancialStatsDto> GetStatsAsync()
    {
        try
        {
            var dailyCache = await _cacheRepository.GetDailyStatsAsync();
            var weeklyCache = await _cacheRepository.GetWeeklyStatsAsync();
            var monthlyCache = await _cacheRepository.GetMonthlyStatsAsync();

            var dayStats = BuildDayStats(dailyCache);
            var weekStats = BuildWeekStats(weeklyCache);
            var monthStats = BuildMonthStats(monthlyCache);

            return new FinancialStatsDto
            {
                Day = dayStats,
                Week = weekStats,
                Month = monthStats
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build financial stats from cache");
            return GetEmptyStats();
        }
    }

    private ChartDataDto BuildDayStats(List<DailyStatsCache> cacheRows)
    {
        var persianDays = new[] { "شنبه", "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه" };
        var requests = new long[7];
        var payments = new long[7];

        foreach (var row in cacheRows)
        {
            if (row.DayIndex >= 0 && row.DayIndex < 7)
            {
                requests[row.DayIndex] = row.Requests;
                payments[row.DayIndex] = row.Payments;
            }
        }

        return new ChartDataDto
        {
            Labels = persianDays.ToList(),
            Requests = requests.ToList(),
            Payments = payments.ToList()
        };
    }

    private ChartDataDto BuildWeekStats(List<WeeklyStatsCache> cacheRows)
    {
        var labels = new List<string>();
        var requests = new List<long>();
        var payments = new List<long>();

        foreach (var row in cacheRows.OrderBy(r => r.WeekPosition))
        {
            labels.Add(row.WeekLabel);
            requests.Add(row.Requests);
            payments.Add(row.Payments);
        }

        if (labels.Count == 0)
        {
            labels = new List<string> { "هفته ۱", "هفته ۲", "هفته ۳", "هفته ۴" };
            requests = new List<long> { 0, 0, 0, 0 };
            payments = new List<long> { 0, 0, 0, 0 };
        }

        return new ChartDataDto
        {
            Labels = labels,
            Requests = requests,
            Payments = payments
        };
    }

    private ChartDataDto BuildMonthStats(List<MonthlyStatsCache> cacheRows)
    {
        var persianMonths = new[]
        {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };
        var requests = new long[12];
        var payments = new long[12];

        foreach (var row in cacheRows)
        {
            if (row.MonthIndex >= 0 && row.MonthIndex < 12)
            {
                requests[row.MonthIndex] = row.Requests;
                payments[row.MonthIndex] = row.Payments;
            }
        }

        return new ChartDataDto
        {
            Labels = persianMonths.ToList(),
            Requests = requests.ToList(),
            Payments = payments.ToList()
        };
    }

    private FinancialStatsDto GetEmptyStats()
    {
        var persianDays = new[] { "شنبه", "یکشنبه", "دنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه" };
        var persianMonths = new[]
        {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };
        return new FinancialStatsDto
        {
            Day = new ChartDataDto { Labels = persianDays.ToList(), Requests = new long[7].ToList(), Payments = new long[7].ToList() },
            Week = new ChartDataDto { Labels = new List<string> { "هفته ۱", "هفته ۲", "هفته ۳", "هفته ۴" }, Requests = new long[4].ToList(), Payments = new long[4].ToList() },
            Month = new ChartDataDto { Labels = persianMonths.ToList(), Requests = new long[12].ToList(), Payments = new long[12].ToList() }
        };
    }
}