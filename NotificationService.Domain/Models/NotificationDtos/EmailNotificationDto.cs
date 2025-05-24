using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Models.NotificationDtos;

public class EmailNotificationDto : BaseNotificationDto
{
    public List<string> Emails { get; set; }
    
    
    public string? FromEmailAddress { get; set; }
    public string? EmailPassword { get; set; }
}