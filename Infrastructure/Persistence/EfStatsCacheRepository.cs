using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Domain.Entities;
using AvalWebBackend.Application.Common.Helpers;

namespace AvalWebBackend.Infrastructure.Persistence;

public class EfStatsCacheRepository : IStatsCacheRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<EfStatsCacheRepository> _logger;

     
    private static readonly string[] PersianDayNames = { "شنبه", "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه" };
    private static readonly string[] PersianMonthNames = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };

    public EfStatsCacheRepository(AppDbContext context, ILogger<EfStatsCacheRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    
    public async Task<List<DailyStatsCache>> GetDailyStatsAsync() =>
        await _context.DailyStatsCache.ToListAsync();
    public async Task<List<WeeklyStatsCache>> GetWeeklyStatsAsync() =>
        await _context.WeeklyStatsCache.OrderBy(w => w.WeekPosition).ToListAsync();
    public async Task<List<MonthlyStatsCache>> GetMonthlyStatsAsync() =>
        await _context.MonthlyStatsCache.OrderBy(m => m.MonthIndex).ToListAsync();

    
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
                Requests = 0,
                Payments = 0,
                UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };
            _context.DailyStatsCache.Add(row);
        }

        if (transaction.Type == "request")
            row.Requests += transaction.Amount;
        else
            row.Payments += transaction.Amount;
        row.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        
        UpdateWeeklyForDate(txDate);
        UpdateMonthlyForDate(txDate);
    }

    
    public void UpsertWeeklyCache(Transaction transaction, DateTime txDate) { }
    public void UpsertMonthlyCache(Transaction transaction, DateTime txDate) { }

    
    public async Task ArchiveAndResetIfNewPeriodAsync(DateTime txDate)
    {
        var pc = new System.Globalization.PersianCalendar();
        var startOfWeek = PersianDateHelper.GetStartOfPersianWeek(txDate);
        string currentWeekStartStr = ToPersianDateString(startOfWeek);

        
        bool weekAlreadyExists = await _context.DailyStatsCache
            .AnyAsync(d => d.DayIndex == 0 && d.Date == currentWeekStartStr);

        if (!weekAlreadyExists)
        {
            _logger.LogInformation("New Persian week detected (starts {Start}) – archiving previous week", currentWeekStartStr);

            
            var oldDaily = await _context.DailyStatsCache.ToListAsync();
            if (oldDaily.Any())
            {
                long reqSum = oldDaily.Sum(d => d.Requests);
                long paySum = oldDaily.Sum(d => d.Payments);
                var previousWeekStart = startOfWeek.AddDays(-7).Date;
                _context.WeeklyArchives.Add(new WeeklyArchives
                {
                    WeekStart = ToPersianDateString(previousWeekStart),
                    WeekLabel = $"هفته منتهی به {ToPersianDateString(previousWeekStart.AddDays(6))}",
                    Requests = reqSum,
                    Payments = paySum,
                    ArchivedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }

            
            _context.DailyStatsCache.RemoveRange(oldDaily);
            for (int i = 0; i < 7; i++)
            {
                var dayDate = startOfWeek.AddDays(i);
                _context.DailyStatsCache.Add(new DailyStatsCache
                {
                    DayIndex = i,
                    Date = ToPersianDateString(dayDate),
                    Requests = 0,
                    Payments = 0,
                    UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }

            
            var weeks = await _context.WeeklyStatsCache.OrderBy(w => w.WeekPosition).ToListAsync();

            var oldestWeek = weeks.FirstOrDefault(w => w.WeekPosition == 1);
            if (oldestWeek != null)
            {
                _context.WeeklyStatsCache.Remove(oldestWeek);
            }

            foreach (var w in weeks.Where(w => w.WeekPosition > 1))
            {
                w.WeekPosition--;
            }

            var updatedWeeks = weeks.Where(w => w.WeekPosition >= 1 && w.WeekPosition <= 3).ToList();
            string[] relativeLabels = { "سه هفته پیش", "دو هفته پیش", "یک هفته پیش" };
            for (int i = 0; i < 3; i++)
            {
                var week = updatedWeeks.FirstOrDefault(w => w.WeekPosition == i + 1);
                if (week != null) week.WeekLabel = relativeLabels[i];
            }

            
            _context.WeeklyStatsCache.Add(new WeeklyStatsCache
            {
                WeekPosition = 4,
                WeekStart = currentWeekStartStr,
                WeekLabel = "این هفته",
                Requests = 0,
                Payments = 0,
                UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        
        int year = pc.GetYear(txDate);
        int month = pc.GetMonth(txDate);  
        bool monthExists = await _context.MonthlyStatsCache
            .AnyAsync(m => m.Year == year && m.MonthIndex == month - 1);

        if (!monthExists)
        {
            _logger.LogInformation("New Persian month detected ({Year}/{Month}) – resetting monthly cache", year, month);

            var oldMonths = await _context.MonthlyStatsCache.ToListAsync();
            foreach (var old in oldMonths)
            {
                _context.MonthlyArchives.Add(new MonthlyArchives
                {
                    MonthStart = $"{old.Year}/{old.MonthIndex + 1:D2}/01",
                    MonthLabel = old.MonthLabel,
                    Year = old.Year,
                    Requests = old.Requests,
                    Payments = old.Payments,
                    ArchivedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            _context.MonthlyStatsCache.RemoveRange(oldMonths);

            for (int i = 0; i < 12; i++)
            {
                _context.MonthlyStatsCache.Add(new MonthlyStatsCache
                {
                    MonthIndex = i,
                    Year = year,
                    MonthLabel = PersianMonthNames[i],
                    Requests = 0,
                    Payments = 0,
                    UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    

    
    private void UpdateWeeklyForDate(DateTime txDate)
    {
        var today = DateTime.Today;
        int weekPos = GetWeekPosition(txDate, today);
        if (weekPos < 1 || weekPos > 4) return;

        var weekRow = _context.WeeklyStatsCache.FirstOrDefault(w => w.WeekPosition == weekPos);
        if (weekRow == null) return;

        var daily = _context.DailyStatsCache.ToList();
        var startOfWeek = PersianDateHelper.GetStartOfPersianWeek(txDate);
        var endOfWeek = startOfWeek.AddDays(6);
        var relevantDaily = daily.Where(d =>
        {
            if (!PersianDateHelper.TryParsePersianDate(d.Date, out var dt))
                return false;
            return dt >= startOfWeek && dt <= endOfWeek;
        }).ToList();

        weekRow.Requests = relevantDaily.Sum(d => d.Requests);
        weekRow.Payments = relevantDaily.Sum(d => d.Payments);
        weekRow.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    }

    
    private void UpdateMonthlyForDate(DateTime txDate)
    {
        var pc = new System.Globalization.PersianCalendar();
        int year = pc.GetYear(txDate);
        int monthIdx = pc.GetMonth(txDate) - 1;
        var monthRow = _context.MonthlyStatsCache.FirstOrDefault(m => m.Year == year && m.MonthIndex == monthIdx);
        if (monthRow == null) return;

        var daily = _context.DailyStatsCache.ToList();
        var relevantDaily = daily.Where(d =>
        {
            if (!PersianDateHelper.TryParsePersianDate(d.Date, out var dt))
                return false;
            return pc.GetYear(dt) == year && pc.GetMonth(dt) == monthIdx + 1;
        }).ToList();

        monthRow.Requests = relevantDaily.Sum(d => d.Requests);
        monthRow.Payments = relevantDaily.Sum(d => d.Payments);
        monthRow.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    }


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