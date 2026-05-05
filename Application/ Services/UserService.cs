using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.Common.Exceptions;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto> GetUserByIdAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id)
            ?? throw new NotFoundException("کاربر", id);
        return MapToDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
    {
        // Check duplicate username
        if (await _userRepository.UsernameExistsAsync(request.Username))
            throw new BusinessRuleException("این نام کاربری قبلاً انتخاب شده است");

        var newUser = new User
        {
            Id = Guid.NewGuid().ToString()[..8],
            Username = request.Username,
            Password = request.Password,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            SerialNumber = new Random().Next(10000000, 99999999).ToString(),
            RoleKey = "USER"
        };

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();

        return MapToDto(newUser);
    }

    public async Task<UserDto> UpdateUserAsync(string id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(id)
            ?? throw new NotFoundException("کاربر", id);

        if (request.FullName != null) user.FullName = request.FullName;
        if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;

        await _userRepository.UpdateUserAsync(user);
        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id)
            ?? throw new NotFoundException("کاربر", id);

        await _userRepository.DeleteUserAsync(id);
        await _userRepository.SaveChangesAsync();
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        FullName = user.FullName,
        PhoneNumber = user.PhoneNumber ?? "",
        SerialNumber = user.SerialNumber,
        RoleKey = user.RoleKey ?? "USER"
    };
}