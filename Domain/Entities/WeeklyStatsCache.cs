namespace AvalWebBackend.Domain.Entities;

public class WeeklyStatsCache
{
    public int Id { get; set; }
    public int WeekPosition { get; set; }   // 1..4
    public string WeekStart { get; set; } = string.Empty;
    public string WeekLabel { get; set; } = string.Empty;
    public long Requests { get; set; }
    public long Payments { get; set; }
    public string UpdatedAt { get; set; } = string.Empty;
}