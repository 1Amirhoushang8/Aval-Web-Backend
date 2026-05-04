using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using System.Linq;
using System.Threading.Tasks;

namespace AvalWebBackend.Infrastructure.Filters;

public class ValidateCsrfFilter : IAsyncActionFilter
{
    private readonly IAntiforgery _antiforgery;

    public ValidateCsrfFilter(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        
        if (context.ActionDescriptor.EndpointMetadata.Any(
                em => em is IgnoreAntiforgeryTokenAttribute))
        {
            await next();
            return;
        }

        var httpContext = context.HttpContext;
        var method = httpContext.Request.Method;

        if (method == HttpMethods.Post || method == HttpMethods.Put ||
            method == HttpMethods.Delete || method == HttpMethods.Patch)
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(httpContext);
            }
            catch (AntiforgeryValidationException)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    message = "توکن CSRF نامعتبر است یا وجود ندارد."
                });
                return;
            }
        }

        await next();
    }
}