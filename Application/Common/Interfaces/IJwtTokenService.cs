namespace AvalWebBackend.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(string userId, string role, string? fullName = null);
}