namespace AvalWebBackend.Application.DTOs;

public class MessageDto
{
    public string Id { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public bool IsRead { get; set; }
}