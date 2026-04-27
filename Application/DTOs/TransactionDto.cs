namespace AvalWebBackend.Application.DTOs;

public class TransactionDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string TransactionDate { get; set; } = string.Empty;
    public string? Source { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}