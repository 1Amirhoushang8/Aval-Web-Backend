using Microsoft.EntityFrameworkCore;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Infrastructure.Persistence;

public class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public EfUserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<Admin?> GetAdminByUsernameAsync(string username) =>
        await _context.Admins.FirstOrDefaultAsync(a => a.Username == username);

    public async Task<bool> UsernameExistsAsync(string username) =>
        await _context.Users.AnyAsync(u => u.Username == username);

    public async Task<bool> PasswordExistsAsync(string password) =>
        await _context.Users.AnyAsync(u => u.Password == password);

    public async Task<bool> UserExistsByIdAsync(string id) =>
        await _context.Users.AnyAsync(u => u.Id == id);

    public async Task<bool> AdminExistsByIdAsync(string id) =>
        await _context.Admins.AnyAsync(a => a.Id == id);

    public async Task<List<User>> GetAllUsersAsync() =>
        await _context.Users.ToListAsync();

    public async Task<User?> GetUserByIdAsync(string id) =>
        await _context.Users.FindAsync(id);

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public async Task UpdateUserAsync(User user) =>
        _context.Users.Update(user);

    public async Task DeleteUserAsync(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null) _context.Users.Remove(user);
    }

    public async Task<bool> IsSerialNumberDuplicateAsync(string serialNumber, string? excludeUserId = null)
    {
        var query = _context.Users.Where(u => u.SerialNumber == serialNumber);
        if (!string.IsNullOrEmpty(excludeUserId))
            query = query.Where(u => u.Id != excludeUserId);
        return await query.AnyAsync();
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}