namespace AvalWebBackend.Application.DTOs;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public string MonthlyPayment { get; set; } = string.Empty;
    public int? TotalMonths { get; set; }
}