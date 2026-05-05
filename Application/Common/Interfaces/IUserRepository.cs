using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Common.Interfaces;

public interface IUserRepository
{
    
    Task<User?> GetByUsernameAsync(string username);
    Task<Admin?> GetAdminByUsernameAsync(string username);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> PasswordExistsAsync(string password);
    Task<bool> UserExistsByIdAsync(string userId);
    Task<bool> AdminExistsByIdAsync(string adminId);

    
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(string id);
    Task AddAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(string id);
    Task<bool> IsSerialNumberDuplicateAsync(string serialNumber, string? excludeUserId = null);
    Task SaveChangesAsync();
}