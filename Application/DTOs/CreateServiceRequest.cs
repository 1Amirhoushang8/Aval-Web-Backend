using System.ComponentModel.DataAnnotations;

namespace AvalWebBackend.Application.DTOs;

public class CreateServiceRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    [Required]
    public string SerialNumber { get; set; } = string.Empty;
    public string? ServiceName { get; set; }
    public string? Price { get; set; }
    public string Status { get; set; } = "درحال-انجام";
    public string PaymentType { get; set; } = "پرداخت-تکی";
    public string? MonthlyPayment { get; set; }
    public int? TotalMonths { get; set; }
}