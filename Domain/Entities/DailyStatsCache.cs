namespace AvalWebBackend.Domain.Entities;

public class DailyStatsCache
{
    public int Id { get; set; }
    public int DayIndex { get; set; }       // 0=Sat … 6=Fri
    public string Date { get; set; } = string.Empty;
    public long Requests { get; set; }
    public long Payments { get; set; }
    public string UpdatedAt { get; set; } = string.Empty;
}