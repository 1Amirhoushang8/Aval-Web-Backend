using Microsoft.EntityFrameworkCore;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Domain.Entities;
using AvalWebBackend.Application.Common.Helpers;   

namespace AvalWebBackend.Infrastructure.Persistence;

public class EfStatsCacheRepository : IStatsCacheRepository
{
    private readonly AppDbContext _context;

    public EfStatsCacheRepository(AppDbContext context)
    {
        _context = context;
    }

    // ---------- Read methods (unchanged) ----------
    public async Task<List<DailyStatsCache>> GetDailyStatsAsync() =>
        await _context.DailyStatsCache.ToListAsync();

    public async Task<List<WeeklyStatsCache>> GetWeeklyStatsAsync() =>
        await _context.WeeklyStatsCache.ToListAsync();

    public async Task<List<MonthlyStatsCache>> GetMonthlyStatsAsync() =>
        await _context.MonthlyStatsCache.ToListAsync();

    // ---------- Upsert methods ----------

    public void UpsertDailyCache(Transaction transaction, DateTime txDate)
    {
        int dayIndex = PersianDateHelper.GetPersianDayIndex(txDate);
        var row = _context.DailyStatsCache.FirstOrDefault(d => d.DayIndex == dayIndex);

        if (row == null)
        {
            row = new DailyStatsCache
            {
                DayIndex = dayIndex,
                Date = transaction.TransactionDate,
                Requests = transaction.Type == "request" ? transaction.Amount : 0,
                Payments = transaction.Type == "payment" ? transaction.Amount : 0,
                UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };
            _context.DailyStatsCache.Add(row);
        }
        else
        {
            if (transaction.Type == "request") row.Requests += transaction.Amount;
            else row.Payments += transaction.Amount;
            row.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    public void UpsertWeeklyCache(Transaction transaction, DateTime txDate)
    {
        var today = DateTime.Today;
        int weekPos = GetWeekPosition(txDate, today);
        if (weekPos < 1 || weekPos > 4) return;

        var row = _context.WeeklyStatsCache.FirstOrDefault(w => w.WeekPosition == weekPos);
        if (row == null)
        {
            var weekStart = PersianDateHelper.GetStartOfPersianWeek(txDate);
            row = new WeeklyStatsCache
            {
                WeekPosition = weekPos,
                WeekStart = ToPersianDateString(weekStart),
                WeekLabel = $"هفته {weekPos}",
                Requests = transaction.Type == "request" ? transaction.Amount : 0,
                Payments = transaction.Type == "payment" ? transaction.Amount : 0,
                UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };
            _context.WeeklyStatsCache.Add(row);
        }
        else
        {
            if (transaction.Type == "request") row.Requests += transaction.Amount;
            else row.Payments += transaction.Amount;
            row.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    public void UpsertMonthlyCache(Transaction transaction, DateTime txDate)
    {
        var pc = new System.Globalization.PersianCalendar();
        int year = pc.GetYear(txDate);
        int monthIndex = pc.GetMonth(txDate) - 1; // 0..11

        var row = _context.MonthlyStatsCache.FirstOrDefault(m => m.Year == year && m.MonthIndex == monthIndex);
        string[] monthNames = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };

        if (row == null)
        {
            row = new MonthlyStatsCache
            {
                MonthIndex = monthIndex,
                Year = year,
                MonthLabel = monthNames[monthIndex],
                Requests = transaction.Type == "request" ? transaction.Amount : 0,
                Payments = transaction.Type == "payment" ? transaction.Amount : 0,
                UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };
            _context.MonthlyStatsCache.Add(row);
        }
        else
        {
            if (transaction.Type == "request") row.Requests += transaction.Amount;
            else row.Payments += transaction.Amount;
            row.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    // ---------- Helpers ----------
    private static int GetWeekPosition(DateTime txDate, DateTime today)
    {
        var startOfTxWeek = PersianDateHelper.GetStartOfPersianWeek(txDate);
        var startOfThisWeek = PersianDateHelper.GetStartOfPersianWeek(today);
        int diffWeeks = (int)(startOfThisWeek - startOfTxWeek).TotalDays / 7;
        return 4 - diffWeeks; 
    }

    private static string ToPersianDateString(DateTime date)
    {
        var pc = new System.Globalization.PersianCalendar();
        return $"{pc.GetYear(date)}/{pc.GetMonth(date):00}/{pc.GetDayOfMonth(date):00}";
    }
}