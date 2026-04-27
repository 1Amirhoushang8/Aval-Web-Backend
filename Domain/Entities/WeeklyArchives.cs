namespace AvalWebBackend.Domain.Entities;

public class WeeklyArchives
{
    public int Id { get; set; }
    public string WeekStart { get; set; } = string.Empty;      
    public string WeekLabel { get; set; } = string.Empty;     
    public long Requests { get; set; }
    public long Payments { get; set; }
    public string ArchivedAt { get; set; } = string.Empty;
}