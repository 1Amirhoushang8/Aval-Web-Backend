namespace AvalWebBackend.Domain.Entities;

public class MonthlyArchives
{
	public int Id { get; set; }
	public string MonthStart { get; set; } = string.Empty;      
	public string MonthLabel { get; set; } = string.Empty;     
	public int Year { get; set; }
	public long Requests { get; set; }
	public long Payments { get; set; }
	public string ArchivedAt { get; set; } = string.Empty;
}