using System.ComponentModel.DataAnnotations;

namespace AvalWebBackend.Application.DTOs;

public class CreateUserRequest
{
    [Required(ErrorMessage = "شماره فاکتور الزامی است")]
    public string SerialNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام کامل الزامی است")]
    [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "فقط حروف فارسی مجاز است")]
    public string FullName { get; set; } = string.Empty;

    [RegularExpression(@"^0?9\d{9}$", ErrorMessage = "شماره تماس نامعتبر است")]
    public string? PhoneNumber { get; set; }

    public string? Service { get; set; }
    public string? Price { get; set; }

    [RegularExpression("^(درحال-انجام|پرداخت-شده|لغو-شده)$", ErrorMessage = "وضعیت نامعتبر است")]
    public string Status { get; set; } = "درحال-انجام";

    [RegularExpression("^(پرداخت-تکی|پرداخت-دوره-ای)$", ErrorMessage = "نوع پرداخت نامعتبر است")]
    public string PaymentType { get; set; } = "پرداخت-تکی";

    public string? MonthlyPayment { get; set; }
    public int? TotalMonths { get; set; }
}