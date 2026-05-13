using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;

namespace AvalWebBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CsrfController : ControllerBase
{
    private readonly IAntiforgery _antiforgery;

    public CsrfController(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    [HttpGet]
    public IActionResult GetToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        // Set the cookie with the same token, Path="/" so it's sent for all paths
        Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });
        return Ok(new { token = tokens.RequestToken });
    }
}