using NotificationService.WebApi.Middlewares;

namespace NotificationService.WebApi.Extensions;

public static class ExceptionHandlerMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        return app;
    }
}