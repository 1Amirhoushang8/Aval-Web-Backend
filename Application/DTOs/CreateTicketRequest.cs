using System.ComponentModel.DataAnnotations;

namespace AvalWebBackend.Application.DTOs;

public class CreateTicketRequest
{
    [Required(ErrorMessage = "شناسه کاربر الزامی است")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "عنوان الزامی است")]
    [MaxLength(200, ErrorMessage = "عنوان نباید بیشتر از ۲۰۰ کاراکتر باشد")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "توضیحات کوتاه نباید بیشتر از ۵۰۰ کاراکتر باشد")]
    public string ShortDetail { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public object? File { get; set; }
}