using System.ComponentModel.DataAnnotations;

namespace AvalWebBackend.Application.DTOs;

public class UpdateTicketRequest
{
    [Required(ErrorMessage = "عنوان الزامی است")]
    [MaxLength(200, ErrorMessage = "عنوان نباید بیشتر از ۲۰۰ کاراکتر باشد")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "توضیحات کوتاه نباید بیشتر از ۵۰۰ کاراکتر باشد")]
    public string ShortDetail { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "وضعیت الزامی است")]
    [RegularExpression("^(pending|in-progress|answered)$", ErrorMessage = "وضعیت نامعتبر است")]
    public string Status { get; set; } = string.Empty;

    public string? AdminResponse { get; set; }

    public object? File { get; set; }
}