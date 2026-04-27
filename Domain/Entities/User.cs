namespace AvalWebBackend.Domain.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string? Service { get; set; }
    public string? Price { get; set; }
    public string Status { get; set; } = "درحال-انجام";
    public string PaymentType { get; set; } = "پرداخت-تکی";
    public string? RoleKey { get; set; } = "USER";
    public string? MonthlyPayment { get; set; }
    public int? TotalMonths { get; set; }
}