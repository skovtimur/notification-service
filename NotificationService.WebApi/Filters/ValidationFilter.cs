using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NotificationService.WebApi.Filters;

public class ValidationFilter(ILogger<ValidationFilter> logger) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid == false)
        {
            logger.LogTrace("Validation filter failed");
            context.Result = new BadRequestObjectResult("Validation failed!");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ValidationFilterAttribute() : ServiceFilterAttribute(typeof(ValidationFilter));