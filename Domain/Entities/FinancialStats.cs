namespace AvalWebBackend.Models;


public class FinancialStats
{
    public ChartData Day { get; set; } = new();
    public ChartData Week { get; set; } = new();
    public ChartData Month { get; set; } = new();
}