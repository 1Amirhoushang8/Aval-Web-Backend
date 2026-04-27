using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Infrastructure.Persistence;

public class JsonMessageRepository : IMessageRepository
{
    private readonly JsonDataService _dataService;

    public JsonMessageRepository(JsonDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<List<Message>> GetByTicketIdAsync(string ticketId)
    {
        var db = await _dataService.ReadAsync();
        return db.Messages
            .Where(m => m.TicketId == ticketId)
            .OrderBy(m => m.Timestamp)
            .ToList();
    }

    public async Task<Message?> GetByIdAsync(string messageId)
    {
        var db = await _dataService.ReadAsync();
        return db.Messages.FirstOrDefault(m => m.Id == messageId);
    }

    public async Task AddAsync(Message message)
    {
        var db = await _dataService.ReadAsync();
        db.Messages.Add(message);
        await _dataService.WriteAsync(db);
    }

    public async Task UpdateAsync(Message message)
    {
        var db = await _dataService.ReadAsync();
        var index = db.Messages.FindIndex(m => m.Id == message.Id);
        if (index >= 0)
        {
            db.Messages[index] = message;
            await _dataService.WriteAsync(db);
        }
    }

    public Task SaveChangesAsync()
    {
        
        return Task.CompletedTask;
    }
}