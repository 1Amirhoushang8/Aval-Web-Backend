using Microsoft.AspNetCore.Mvc;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.DTOs;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;

namespace AvalWebBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    [EnableRateLimiting("LoginPolicy")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var data = await _authService.LoginAsync(request.Username, request.Password);

        string token = ExtractToken(data);
        string userJson = ExtractUserJson(data);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddHours(2),
            Path = "/"
        };

        Response.Cookies.Append("access_token", token, cookieOptions);

        var userObject = JsonSerializer.Deserialize<object>(userJson);
        return Ok(new { user = userObject });
    }

    [HttpPost("register")]
    [EnableRateLimiting("RegisterPolicy")]


    [IgnoreAntiforgeryToken]


    public async Task<IActionResult> Register([FromBody] SignupRequest request)
    {
        var userId = await _authService.RegisterAsync(request);
        return Ok(new { userId });
    }

    private static string ExtractToken(object loginResult)
    {
        var json = JsonSerializer.Serialize(loginResult);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("token").GetString()!;
    }

    private static string ExtractUserJson(object loginResult)
    {
        var json = JsonSerializer.Serialize(loginResult);
        using var doc = JsonDocument.Parse(json);
        var userElement = doc.RootElement.GetProperty("user");
        return userElement.GetRawText();
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}