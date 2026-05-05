namespace AvalWebBackend.Application.DTOs;

public class UpdateServiceRequest
{
    public string? SerialNumber { get; set; }
    public string? ServiceName { get; set; }
    public string? Price { get; set; }
    public string? Status { get; set; }
    public string? PaymentType { get; set; }
    public string? MonthlyPayment { get; set; }
    public int? TotalMonths { get; set; }
}