namespace AvalWebBackend.Domain.Entities;

public class Database
{
    public List<Admin> Admins { get; set; } = new();
    public List<User> Users { get; set; } = new();
    public List<Ticket> Tickets { get; set; } = new();
    public List<Message> Messages { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();
}