using System.ComponentModel.DataAnnotations;

namespace AvalWebBackend.Application.DTOs;

public class SendMessageRequest
{
    [Required(ErrorMessage = "شناسه تیکت الزامی است")]
    public string TicketId { get; set; } = string.Empty;

    [Required(ErrorMessage = "شناسه فرستنده الزامی است")]
    public string SenderId { get; set; } = string.Empty;

    [Required(ErrorMessage = "متن پیام الزامی است")]
    [MaxLength(2000, ErrorMessage = "متن پیام نباید بیشتر از ۲۰۰۰ کاراکتر باشد")]
    public string MessageText { get; set; } = string.Empty;
}