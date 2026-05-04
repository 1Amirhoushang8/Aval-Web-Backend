using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AvalWebBackend.Infrastructure.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    private const string ApiKeyHeaderName = "X-Api-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "کلید API ارسال نشده است." });
            return;
        }

        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var configuredApiKey = configuration.GetValue<string>("ExternalApiKey");

        if (string.IsNullOrEmpty(configuredApiKey) || !configuredApiKey.Equals(extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "کلید API نامعتبر است." });
            return;
        }

        await next();
    }
}