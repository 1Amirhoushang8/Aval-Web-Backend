using System.ComponentModel.DataAnnotations;

namespace AvalWebBackend.Application.DTOs;

public class UpdateUserRequest
{
    [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "فقط حروف فارسی مجاز است")]
    public string? FullName { get; set; }

    [RegularExpression(@"^0?9\d{9}$", ErrorMessage = "شماره تماس نامعتبر است")]
    public string? PhoneNumber { get; set; }

    
}