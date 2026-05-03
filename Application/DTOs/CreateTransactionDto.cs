using System.ComponentModel.DataAnnotations;

namespace AvalWebBackend.Application.DTOs;

public class CreateTransactionDto
{
    [Required(ErrorMessage = "نوع تراکنش الزامی است")]
    [RegularExpression("^(request|payment)$", ErrorMessage = "نوع تراکنش باید request یا payment باشد")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "مبلغ الزامی است")]
    [Range(1, long.MaxValue, ErrorMessage = "مبلغ باید بیشتر از صفر باشد")]
    public long Amount { get; set; }

    [Required(ErrorMessage = "تاریخ تراکنش الزامی است")]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "فرمت تاریخ باید yyyy-MM-dd باشد")]
    public string TransactionDate { get; set; } = string.Empty;

    public string? Source { get; set; }

    public string? IdempotencyKey { get; set; }
}