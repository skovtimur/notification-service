using NotificationService.Domain.Models;
using NotificationService.Domain.Models.NotificationDtos;
using NotificationService.Domain.ValueObjects;
using NotificationService.WebApi.Queries;

namespace NotificationService.WebApi.Extensions;

public static class MapperExtensions
{
    public static async Task<EmailNotificationDto> ToEmailNotificationDto(this EmailNotifyQuery query)
    {
        FileDto? fileDto = null;

        if (query.File != null)
            fileDto = await query.File.ToFileDto();

        return new EmailNotificationDto
        {
            Emails = query.Emails,
            File = fileDto,
            Text = query.Text,
            Priority = query.Priority,
            WillDoAt = query.WillDoAt,
            FromEmailAddress = query.FromEmailAddress,
            EmailPassword = query.EmailPassword
        };
    }

    public static async Task<TelegramNotificationDto> ToTelegramNotificationDto(this TelegramNotifyQuery query,
        List<TelegramIdValueObject> telegramIds)
    {
        FileDto? fileDto = null;

        if (query.File != null)
            fileDto = await query.File.ToFileDto();


        return new TelegramNotificationDto
        {
            ChatIdValueObjects = telegramIds,
            File = fileDto,
            Text = query.Text,
            Priority = query.Priority,
            WillDoAt = query.WillDoAt,
            BotApiKey = query.BotApiKey
        };
    }
}

public class MappingException(string reason, string message) : Exception
{
    public string ReasonOfFailure { get; } = reason;
    public override string Message { get; } = message;
}