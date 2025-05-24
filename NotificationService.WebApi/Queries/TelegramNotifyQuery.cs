using System.ComponentModel.DataAnnotations;
using NotificationService.Domain.Validators;

namespace NotificationService.WebApi.Queries;

public class TelegramNotifyQuery : BaseNotifyQuery
{
    [Required]
    public List<long> ChatIds { get; set; }
    
    public string BotApiKey { get; set; }
}