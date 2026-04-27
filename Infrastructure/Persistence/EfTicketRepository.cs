using Microsoft.EntityFrameworkCore;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Infrastructure.Persistence;

public class EfTicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;
    public EfTicketRepository(AppDbContext context) => _context = context;

    public async Task<List<Ticket>> GetAllAsync() =>
        await _context.Tickets.ToListAsync();

    public async Task<Ticket?> GetByIdAsync(string id) =>
        await _context.Tickets.FindAsync(id);

    public async Task<Ticket?> GetByIdWithFileAsync(string id) =>
        await _context.Tickets.FindAsync(id);

    public async Task<bool> ExistsAsync(string id) =>
        await _context.Tickets.AnyAsync(t => t.Id == id);

    public async Task AddAsync(Ticket ticket) =>
        await _context.Tickets.AddAsync(ticket);

    public async Task UpdateAsync(Ticket ticket) =>
        _context.Tickets.Update(ticket);

    public async Task DeleteAsync(string id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket != null) _context.Tickets.Remove(ticket);
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}