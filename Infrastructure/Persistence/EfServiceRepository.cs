using Microsoft.EntityFrameworkCore;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Infrastructure.Persistence;

public class EfServiceRepository : IServiceRepository
{
    private readonly AppDbContext _context;
    public EfServiceRepository(AppDbContext context) => _context = context;

    public async Task<List<Service>> GetAllAsync() =>
        await _context.Services.Include(s => s.User).ToListAsync();

    public async Task<List<Service>> GetByUserIdAsync(string userId) =>
        await _context.Services.Where(s => s.UserId == userId).ToListAsync();

    
    public async Task<Service?> GetByIdAsync(string id) =>
        await _context.Services.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);

    public async Task AddAsync(Service service) =>
        await _context.Services.AddAsync(service);

    public async Task UpdateAsync(Service service) =>
        _context.Services.Update(service);

    public async Task DeleteAsync(string id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service != null) _context.Services.Remove(service);
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}