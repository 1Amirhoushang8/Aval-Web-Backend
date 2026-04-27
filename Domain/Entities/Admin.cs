namespace AvalWebBackend.Domain.Entities;

public class Admin
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string RoleKey { get; set; } = "ADMIN";
}