using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AvalWebBackend.Application.Common.Exceptions;

namespace AvalWebBackend.Infrastructure.Filters;

public class DomainExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is NotFoundException notFound)
        {
            context.Result = new NotFoundObjectResult(new { message = notFound.Message });
            context.ExceptionHandled = true;
        }
        else if (context.Exception is BusinessRuleException business)
        {
            context.Result = new BadRequestObjectResult(new { message = business.Message });
            context.ExceptionHandled = true;
        }
        
    }
}