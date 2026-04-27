namespace AvalWebBackend.Models;

public class ChartData
{
    public List<string> Labels { get; set; } = new();
    public List<long> Requests { get; set; } = new();
    public List<long> Payments { get; set; } = new();
    public long? TotalRequests { get; set; }
    public long? TotalPayments { get; set; }
}