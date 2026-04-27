using Microsoft.EntityFrameworkCore;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Infrastructure.Persistence;

public class EfMessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;
    public EfMessageRepository(AppDbContext context) => _context = context;

    public async Task<List<Message>> GetByTicketIdAsync(string ticketId) =>
        await _context.Messages.Where(m => m.TicketId == ticketId).ToListAsync();

    public async Task<Message?> GetByIdAsync(string id) =>
        await _context.Messages.FindAsync(id);

    public async Task AddAsync(Message message) =>
        await _context.Messages.AddAsync(message);

    public async Task UpdateAsync(Message message) =>
        _context.Messages.Update(message);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}