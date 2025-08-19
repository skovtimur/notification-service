using NotificationService.Domain.Exceptions;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;

namespace NotificationService.WebApi.Middlewares;

public class ExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlerMiddleware> logger,
    IApplicationLifetime applicationLifetime)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            if (ex is ICriticalException)
            {
                applicationLifetime.StopApplication();
                return;
            }

            await Handle(context, ex, StatusCodes.Status500InternalServerError,
                "Internal Server Error");
        }
    }

    private async Task Handle(HttpContext context, Exception exception,
        int statusCode, string message)
    {
        var response = context.Response;
        response.StatusCode = statusCode;
        response.ContentType = "application/json";

        logger.LogCritical("An exception occurred on the server, exception mes: {m}", exception.Message);

        var exceptionDto = new
        {
            statusCode = statusCode,
            message = message,
            exceptionMessage = exception.Message
        };
        await response.WriteAsJsonAsync(exceptionDto);
    }
}