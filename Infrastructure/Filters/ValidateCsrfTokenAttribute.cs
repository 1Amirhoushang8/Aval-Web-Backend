using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AvalWebBackend.Infrastructure.Filters;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidateCsrfTokenAttribute : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var method = context.HttpContext.Request.Method.ToUpperInvariant();

        
        if (method == "GET" || method == "HEAD" || method == "OPTIONS" || method == "TRACE")
            return;

        
        if (context.Filters.OfType<IgnoreAntiforgeryTokenAttribute>().Any())
            return;

        var antiforgery = context.HttpContext.RequestServices
                                 .GetRequiredService<IAntiforgery>();

        try
        {
            antiforgery.ValidateRequestAsync(context.HttpContext).GetAwaiter().GetResult();
        }
        catch (AntiforgeryValidationException)
        {
            context.Result = new BadRequestObjectResult(new
            {
                message = "توکن CSRF نامعتبر است یا وجود ندارد."
            });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}