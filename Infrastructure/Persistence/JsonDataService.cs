using System.Text.Json;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Infrastructure.Persistence;

public class JsonDataService
{
    private readonly string _filePath;

    public JsonDataService()
    {
        
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "db.json");
    }

    public async Task<Database> ReadAsync()
    {
        if (!File.Exists(_filePath))
            return new Database();

        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<Database>(json) ?? new Database();
    }

    public async Task WriteAsync(Database db)
    {
        var json = JsonSerializer.Serialize(db, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);
    }
}