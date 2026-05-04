using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AvalWebBackend.Infrastructure.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            var isDevelopment = string.Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                "Development",
                StringComparison.OrdinalIgnoreCase);

            // Both branches produce the same anonymous type: { message, stackTrace }
            var response = isDevelopment
                ? new
                {
                    message = $"سرور با خطای غیرمنتظره مواجه شد: {GetExceptionChain(ex)}",
                    stackTrace = ex.StackTrace
                }
                : new
                {
                    message = "خطای داخلی سرور رخ داد. لطفاً دوباره تلاش کنید.",
                    stackTrace = (string?)null
                };

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json; charset=utf-8";

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }

    private static string GetExceptionChain(Exception ex)
    {
        var messages = new List<string>();
        var current = ex;
        while (current != null)
        {
            messages.Add(current.Message);
            current = current.InnerException;
        }
        return string.Join(" ← ", messages);
    }
}