namespace AvalWebBackend.Application.DTOs;

public class FinancialStatsDto
{
    public ChartDataDto Day { get; set; } = new();
    public ChartDataDto Week { get; set; } = new();
    public ChartDataDto Month { get; set; } = new();
}

public class ChartDataDto
{
    public List<string> Labels { get; set; } = new();
    public List<long> Requests { get; set; } = new();
    public List<long> Payments { get; set; } = new();
}