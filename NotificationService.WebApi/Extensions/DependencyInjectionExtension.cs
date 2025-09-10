using NotificationService.Application.Abstraction;
using NotificationService.Application.BackgroundJobs.Listeners;
using NotificationService.Infrastructure.Services;
using NotificationService.WebApi.Filters;

namespace NotificationService.WebApi.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        // Filters:
        services.AddScoped<ValidationFilter>();

        // Services:
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<ITelegramSender, TelegramSender>();
        services.AddScoped<IRecipientService, RecipientService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<ITelegramSender, TelegramSender>();
        services.AddScoped<ITaskStatusService, TaskStatusService>();

        // Job Listeners:
        services.AddScoped<EmailSenderListener>();
        services.AddScoped<TelegramSenderListener>();

        return services;
    }
}