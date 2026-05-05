using System.ComponentModel.DataAnnotations;

namespace AvalWebBackend.Application.DTOs;

public class CreateUserRequest
{
    [Required(ErrorMessage = "نام کامل الزامی است")]
    [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "فقط حروف فارسی مجاز است")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام کاربری الزامی است")]
    [RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "فقط حروف انگلیسی، اعداد و زیرخط")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [MinLength(6, ErrorMessage = "رمز عبور باید حداقل ۶ کاراکتر باشد")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "شماره تماس الزامی است")]
    [RegularExpression(@"^0?9\d{9}$", ErrorMessage = "شماره تماس نامعتبر است")]
    public string PhoneNumber { get; set; } = string.Empty;

    
}