using System.ComponentModel.DataAnnotations;

namespace NotificationService.WebApi.Queries;

public class TelegramNotifyQuery : BaseNotifyQuery
{
    [Required]
    public required List<long> ChatIdentifies { get; set; }

    public string? BotApiKey { get; set; } = null;
}