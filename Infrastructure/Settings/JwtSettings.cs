namespace AvalWebBackend.Infrastructure.Settings;

public class JwtSettings
{
    public string Issuer { get; set; } = "AvalWebBackend";
    public string Audience { get; set; } = "AvalWebFrontend";
    public string SecretKey { get; set; } = string.Empty;
}