using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure.Database;

namespace NotificationService.WebApi.Extensions;

public static class ApplyMigrationsMiddleware
{
    public static void ApplyMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<MainContext>();

        dbContext.Database.Migrate();
    }
}