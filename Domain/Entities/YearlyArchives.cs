namespace AvalWebBackend.Domain.Entities;

public class YearlyArchives
{
    public int Id { get; set; }
    public int Year { get; set; }
    public long Requests { get; set; }
    public long Payments { get; set; }
    public string ArchivedAt { get; set; } = string.Empty;
}