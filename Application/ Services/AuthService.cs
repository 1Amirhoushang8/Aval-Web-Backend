using System.Text.RegularExpressions;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.Common.Exceptions;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;   

    public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<object> LoginAsync(string username, string password)
    {
       
        var admin = await _userRepository.GetAdminByUsernameAsync(username);
        if (admin != null && admin.Password == password)
        {
            var adminData = new
            {
                id = admin.Id,
                username = admin.Username,
                fullName = admin.FullName,
                roleKey = "ADMIN"
            };
            var token = _jwtTokenService.GenerateToken(admin.Id, "ADMIN", admin.FullName);   
            return new { user = adminData, token };
        }

        
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user != null && user.Password == password)
        {
            var userData = new
            {
                id = user.Id,
                username = user.Username,
                fullName = user.FullName,
                roleKey = user.RoleKey ?? "USER",
                phoneNumber = user.PhoneNumber,
                serialNumber = user.SerialNumber
            };
            var token = _jwtTokenService.GenerateToken(user.Id, userData.roleKey, user.FullName);   
            return new { user = userData, token };
        }

        throw new BusinessRuleException("نام کاربری یا رمز عبور اشتباه است");
    }

    public async Task<string> RegisterAsync(SignupRequest request)
    {
        
        if (string.IsNullOrWhiteSpace(request.FullName) || request.FullName.Length < 4)
            throw new BusinessRuleException("نام و نام خانوادگی باید حداقل ۴ کاراکتر و به زبان فارسی باشد");

        if (!Regex.IsMatch(request.FullName, @"^[\u0600-\u06FF\s]+$"))
            throw new BusinessRuleException("نام و نام خانوادگی باید فقط شامل حروف فارسی باشد");

        if (!Regex.IsMatch(request.Username, @"^[A-Za-z0-9_]+$"))
            throw new BusinessRuleException("نام کاربری باید فقط شامل حروف انگلیسی، اعداد و زیرخط باشد");

        if (request.Password.Length < 6)
            throw new BusinessRuleException("رمز عبور باید حداقل ۶ کاراکتر باشد");

        if (await _userRepository.UsernameExistsAsync(request.Username))
            throw new BusinessRuleException("این نام کاربری قبلاً انتخاب شده است");

        if (await _userRepository.PasswordExistsAsync(request.Password))
            throw new BusinessRuleException("این رمز عبور قبلاً استفاده شده است");

        var newUser = new User
        {
            Id = Guid.NewGuid().ToString()[..8],
            Username = request.Username,
            Password = request.Password,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            SerialNumber = new Random().Next(10000000, 99999999).ToString(),
            Service = null!,
            Price = null,
            Status = "درحال-انجام",
            PaymentType = "پرداخت-تکی",
            RoleKey = "USER",
            MonthlyPayment = null,
            TotalMonths = null
        };

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();

        return newUser.Id;
    }
}