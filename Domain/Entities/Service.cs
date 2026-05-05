namespace AvalWebBackend.Domain.Entities;

public class Service
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public string? ServiceName { get; set; }   
    public string? Price { get; set; }
    public string Status { get; set; } = "درحال-انجام";
    public string PaymentType { get; set; } = "پرداخت-تکی";
    public string? MonthlyPayment { get; set; }
    public int? TotalMonths { get; set; }

    
    public User? User { get; set; }
}