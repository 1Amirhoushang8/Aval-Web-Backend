using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Infrastructure.Persistence;

public class JsonUserRepository : IUserRepository
{
    private readonly JsonDataService _dataService;

    public JsonUserRepository(JsonDataService dataService)
    {
        _dataService = dataService;
    }

    // ------------------- helper reads -------------------
    private async Task<List<User>> GetUsersAsync()
    {
        var db = await _dataService.ReadAsync();
        return db.Users ?? new List<User>();
    }

    private async Task<List<Admin>> GetAdminsAsync()
    {
        var db = await _dataService.ReadAsync();
        return db.Admins ?? new List<Admin>();
    }

    // ------------------- auth / existence -------------------
    public async Task<User?> GetByUsernameAsync(string username)
    {
        var users = await GetUsersAsync();
        return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Admin?> GetAdminByUsernameAsync(string username)
    {
        var admins = await GetAdminsAsync();
        return admins.FirstOrDefault(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        var users = await GetUsersAsync();
        return users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> PasswordExistsAsync(string password)
    {
        var users = await GetUsersAsync();
        return users.Any(u => u.Password == password);
    }

    public async Task<bool> UserExistsByIdAsync(string userId)
    {
        var users = await GetUsersAsync();
        return users.Any(u => u.Id == userId);
    }

    public async Task<bool> AdminExistsByIdAsync(string adminId)
    {
        var admins = await GetAdminsAsync();
        return admins.Any(a => a.Id == adminId);
    }

    // ------------------- CRUD for UserService -------------------
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await GetUsersAsync();
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        var users = await GetUsersAsync();
        return users.FirstOrDefault(u => u.Id == id);
    }

    public async Task AddAsync(User user)
    {
        var db = await _dataService.ReadAsync();
        db.Users.Add(user);
        await _dataService.WriteAsync(db);
    }

    public async Task UpdateUserAsync(User user)
    {
        var db = await _dataService.ReadAsync();
        var index = db.Users.FindIndex(u => u.Id == user.Id);
        if (index >= 0)
        {
            db.Users[index] = user;
            await _dataService.WriteAsync(db);
        }
    }

    public async Task DeleteUserAsync(string id)
    {
        var db = await _dataService.ReadAsync();
        db.Users.RemoveAll(u => u.Id == id);
        await _dataService.WriteAsync(db);
    }

    public async Task<bool> IsSerialNumberDuplicateAsync(string serialNumber, string? excludeUserId = null)
    {
        var users = await GetUsersAsync();
        return users.Any(u => u.SerialNumber == serialNumber && (excludeUserId == null || u.Id != excludeUserId));
    }

    public Task SaveChangesAsync()
    {
        
        return Task.CompletedTask;
    }
}