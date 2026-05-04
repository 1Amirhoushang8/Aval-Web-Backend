using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AvalWebBackend.Infrastructure.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        
        var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userId = context.User?.Identity?.IsAuthenticated == true
            ? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            : "anonymous";

        _logger.LogInformation(
            "→ {Method} {Path} started | IP:{IP} User:{User}",
            context.Request.Method,
            context.Request.Path,
            remoteIp,
            userId);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            var elapsed = stopwatch.ElapsedMilliseconds;

            if (statusCode >= 500)
            {
                _logger.LogError(
                    "← {Method} {Path} responded {StatusCode} in {Elapsed}ms | IP:{IP} User:{User}",
                    context.Request.Method,
                    context.Request.Path,
                    statusCode,
                    elapsed,
                    remoteIp,
                    userId);
            }
            else if (statusCode >= 400)
            {
                _logger.LogWarning(
                    "← {Method} {Path} responded {StatusCode} in {Elapsed}ms | IP:{IP} User:{User}",
                    context.Request.Method,
                    context.Request.Path,
                    statusCode,
                    elapsed,
                    remoteIp,
                    userId);
            }
            else
            {
                _logger.LogInformation(
                    "← {Method} {Path} responded {StatusCode} in {Elapsed}ms",
                    context.Request.Method,
                    context.Request.Path,
                    statusCode,
                    elapsed);
            }
        }
    }
}