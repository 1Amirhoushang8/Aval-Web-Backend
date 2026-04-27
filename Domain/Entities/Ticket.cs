namespace AvalWebBackend.Domain.Entities;

public class Ticket
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ShortDetail { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? AdminResponse { get; set; }
    public object? File { get; set; }
}