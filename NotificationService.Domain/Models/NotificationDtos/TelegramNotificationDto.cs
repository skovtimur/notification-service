using NotificationService.Domain.Entities;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Models.NotificationDtos;

public class TelegramNotificationDto : BaseNotificationDto
{
    public List<TelegramIdValueObject> ChatIdValueObjects { get; set; }
    public string BotApiKey { get; set; }
}