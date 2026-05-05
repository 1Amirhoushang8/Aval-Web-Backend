namespace AvalWebBackend.Domain.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string? RoleKey { get; set; } = "USER";
}