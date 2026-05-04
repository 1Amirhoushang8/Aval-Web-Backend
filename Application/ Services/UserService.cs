using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.Common.Exceptions;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto> GetUserByIdAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
            throw new NotFoundException("کاربر", id);
        return MapToDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
    {
        if (await _userRepository.IsSerialNumberDuplicateAsync(request.SerialNumber))
            throw new BusinessRuleException("شماره فاکتور تکراری است");

        var newUser = new User
        {
            Id = Guid.NewGuid().ToString()[..8],
            SerialNumber = request.SerialNumber,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Service = request.Service,
            Price = request.Price,
            Status = request.Status,
            PaymentType = request.PaymentType,
            MonthlyPayment = request.MonthlyPayment,
            TotalMonths = request.TotalMonths,
            Username = request.FullName,
            Password = "default123",
            RoleKey = "USER"
        };

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();

        return MapToDto(newUser);
    }

    public async Task<UserDto> UpdateUserAsync(string id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
            throw new NotFoundException("کاربر", id);

        if (!string.IsNullOrEmpty(request.SerialNumber) &&
            await _userRepository.IsSerialNumberDuplicateAsync(request.SerialNumber, id))
            throw new BusinessRuleException("شماره فاکتور تکراری است");

        if (request.FullName != null) user.FullName = request.FullName;
        if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;
        if (request.SerialNumber != null) user.SerialNumber = request.SerialNumber;
        if (request.Service != null) user.Service = request.Service;
        if (request.Price != null) user.Price = request.Price;
        if (request.Status != null) user.Status = request.Status;
        if (request.PaymentType != null) user.PaymentType = request.PaymentType;
        if (request.MonthlyPayment != null) user.MonthlyPayment = request.MonthlyPayment;
        if (request.TotalMonths.HasValue) user.TotalMonths = request.TotalMonths.Value;

        await _userRepository.UpdateUserAsync(user);
        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }

    
    public async Task DeleteUserAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
            throw new NotFoundException("کاربر", id);

        await _userRepository.DeleteUserAsync(id);
        await _userRepository.SaveChangesAsync();
    }

    
    public async Task DeleteUserServiceAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
            throw new NotFoundException("کاربر", id);

        
        user.Service = null;
        user.Price = null;
        user.Status = "درحال-انجام";
        user.PaymentType = "پرداخت-تکی";
        user.MonthlyPayment = null;
        user.TotalMonths = null;

        await _userRepository.UpdateUserAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        FullName = user.FullName,
        PhoneNumber = user.PhoneNumber ?? "",
        SerialNumber = user.SerialNumber,
        Service = user.Service ?? "",
        Price = user.Price ?? "",
        Status = user.Status,
        PaymentType = user.PaymentType,
        MonthlyPayment = user.MonthlyPayment ?? "",
        TotalMonths = user.TotalMonths
    };
}