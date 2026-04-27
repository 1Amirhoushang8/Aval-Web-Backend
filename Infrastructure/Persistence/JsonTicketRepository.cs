using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Infrastructure.Persistence;

public class JsonTicketRepository : ITicketRepository
{
    private readonly JsonDataService _dataService;

    public JsonTicketRepository(JsonDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<List<Ticket>> GetAllAsync()
    {
        var db = await _dataService.ReadAsync();
        return db.Tickets ?? new List<Ticket>();
    }

    public async Task<Ticket?> GetByIdAsync(string id)
    {
        var db = await _dataService.ReadAsync();
        return db.Tickets.FirstOrDefault(t => t.Id == id);
    }

    public async Task<Ticket?> GetByIdWithFileAsync(string id)
    {
        
        return await GetByIdAsync(id);
    }


    public async Task<bool> ExistsAsync(string id)            
    {
        var db = await _dataService.ReadAsync();
        return db.Tickets.Any(t => t.Id == id);
    }



    public async Task AddAsync(Ticket ticket)
    {
        var db = await _dataService.ReadAsync();
        db.Tickets.Add(ticket);
        await _dataService.WriteAsync(db);
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        var db = await _dataService.ReadAsync();
        var index = db.Tickets.FindIndex(t => t.Id == ticket.Id);
        if (index >= 0)
        {
            db.Tickets[index] = ticket;
            await _dataService.WriteAsync(db);
        }
    }

    public async Task DeleteAsync(string id)
    {
        var db = await _dataService.ReadAsync();
        db.Tickets.RemoveAll(t => t.Id == id);
        await _dataService.WriteAsync(db);
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask; 
    }
}