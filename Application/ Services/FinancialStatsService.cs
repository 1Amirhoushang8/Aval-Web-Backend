using System.Globalization;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Domain.Entities;
using AvalWebBackend.Application.Common.Helpers;

namespace AvalWebBackend.Application.Services;

public class FinancialStatsService : IFinancialStatsService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<FinancialStatsService> _logger;

    // Inject today's date for testability (default to DateTime.Today)
    private readonly DateTime _today;

    public FinancialStatsService(
        ITransactionRepository transactionRepository,
        ILogger<FinancialStatsService> logger,
        DateTime? today = null)   
    {
        _transactionRepository = transactionRepository;
        _logger = logger;
        _today = today ?? DateTime.Today; // fallback to real today
    }

    public async Task<FinancialStatsDto> GetStatsAsync()
    {
        try
        {
            var transactions = await _transactionRepository.GetAllAsync();
            _logger.LogInformation("Total transactions loaded: {Count}", transactions.Count);

            var dayStats = BuildDayStats(transactions);
            var weekStats = BuildWeekStats(transactions);
            var monthStats = BuildMonthStats(transactions);

            return new FinancialStatsDto
            {
                Day = dayStats,
                Week = weekStats,
                Month = monthStats
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build financial stats");
            return GetEmptyStats();
        }
    }

    // ---------- Factory for empty stats ----------
    private FinancialStatsDto GetEmptyStats()
    {
        var persianDays = new[] { "شنبه", "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه" };
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

    // ---------- Day stats ----------
    private ChartDataDto BuildDayStats(List<Transaction> transactions)
    {
        var persianDays = new[] { "شنبه", "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه" };
        var startOfWeek = PersianDateHelper.GetStartOfPersianWeek(_today);

        var requests = new long[7];
        var payments = new long[7];

        foreach (var tx in transactions)
        {
            if (!PersianDateHelper.TryParsePersianDate(tx.TransactionDate, out var txDate))
            {
                _logger.LogWarning("Failed to parse date: {Date}", tx.TransactionDate);
                continue;
            }

            if (txDate < startOfWeek || txDate > startOfWeek.AddDays(6))
                continue;

            int dayIndex = PersianDateHelper.GetPersianDayIndex(txDate);

            if (tx.Type == "request")
                requests[dayIndex] += tx.Amount;
            else if (tx.Type == "payment")
                payments[dayIndex] += tx.Amount;
        }

        return new ChartDataDto
        {
            Labels = persianDays.ToList(),
            Requests = requests.ToList(),
            Payments = payments.ToList()
        };
    }

    // ---------- Week stats ----------
    private ChartDataDto BuildWeekStats(List<Transaction> transactions)
    {
        var labels = new List<string>();
        var requests = new List<long>();
        var payments = new List<long>();

        for (int i = 3; i >= 0; i--)
        {
            var weekStart = PersianDateHelper.GetStartOfPersianWeek(_today.AddDays(-7 * i));
            var weekEnd = weekStart.AddDays(6);
            labels.Add($"هفته {4 - i}");

            long weekRequests = 0, weekPayments = 0;
            foreach (var tx in transactions)
            {
                if (PersianDateHelper.TryParsePersianDate(tx.TransactionDate, out var txDate) &&
                    txDate >= weekStart && txDate <= weekEnd)
                {
                    if (tx.Type == "request") weekRequests += tx.Amount;
                    else if (tx.Type == "payment") weekPayments += tx.Amount;
                }
            }
            requests.Add(weekRequests);
            payments.Add(weekPayments);
        }

        return new ChartDataDto
        {
            Labels = labels,
            Requests = requests,
            Payments = payments
        };
    }

    // ---------- Month stats ----------
    private ChartDataDto BuildMonthStats(List<Transaction> transactions)
    {
        var persianMonths = new[]
        {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };
        var requests = new long[12];
        var payments = new long[12];

        var pc = new PersianCalendar();
        var currentYear = pc.GetYear(_today);

        foreach (var tx in transactions)
        {
            if (!PersianDateHelper.TryParsePersianDate(tx.TransactionDate, out var txDate))
                continue;

            if (pc.GetYear(txDate) != currentYear)
                continue;

            int month = pc.GetMonth(txDate) - 1;
            if (tx.Type == "request")
                requests[month] += tx.Amount;
            else if (tx.Type == "payment")
                payments[month] += tx.Amount;
        }

        return new ChartDataDto
        {
            Labels = persianMonths.ToList(),
            Requests = requests.ToList(),
            Payments = payments.ToList()
        };
    }
}