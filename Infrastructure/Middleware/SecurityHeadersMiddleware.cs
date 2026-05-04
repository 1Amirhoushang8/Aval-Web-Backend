using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AvalWebBackend.Infrastructure.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        
        headers["X-Content-Type-Options"] = "nosniff";

        
        headers["X-Frame-Options"] = "DENY";

        
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        
        if (!context.Request.IsHttps)
        {
            headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
        }

        
        headers["X-Download-Options"] = "noopen";

        
        headers["X-Permitted-Cross-Domain-Policies"] = "none";

        
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), interest-cohort=()";

        
        headers["Content-Security-Policy"] =
            "default-src 'self'; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
            "font-src 'self' https://fonts.gstatic.com; " +
            "img-src 'self' data:; " +
            "connect-src 'self' https://localhost:7208; " +
            "frame-ancestors 'none'; " +
            "base-uri 'self'; " +
            "form-action 'self';";

        await _next(context);
    }
}