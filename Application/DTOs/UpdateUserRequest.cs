using System.ComponentModel.DataAnnotations;

namespace AvalWebBackend.Application.DTOs;

public class UpdateUserRequest
{
    [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "فقط حروف فارسی مجاز است")]
    public string? FullName { get; set; }

    [RegularExpression(@"^0?9\d{9}$", ErrorMessage = "شماره تماس نامعتبر است")]
    public string? PhoneNumber { get; set; }

    public string? SerialNumber { get; set; }
    public string? Service { get; set; }
    public string? Price { get; set; }

    [RegularExpression("^(درحال-انجام|پرداخت-شده|لغو-شده)$", ErrorMessage = "وضعیت نامعتبر است")]
    public string? Status { get; set; }

    [RegularExpression("^(پرداخت-تکی|پرداخت-دوره-ای)$", ErrorMessage = "نوع پرداخت نامعتبر است")]
    public string? PaymentType { get; set; }

    public string? MonthlyPayment { get; set; }
    public int? TotalMonths { get; set; }
}