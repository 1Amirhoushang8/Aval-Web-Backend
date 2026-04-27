namespace AvalWebBackend.Domain.Entities;

public class MonthlyStatsCache
{
    public int Id { get; set; }
    public int MonthIndex { get; set; }     // 0..11
    public int Year { get; set; }
    public string MonthLabel { get; set; } = string.Empty;
    public long Requests { get; set; }
    public long Payments { get; set; }
    public string UpdatedAt { get; set; } = string.Empty;
}