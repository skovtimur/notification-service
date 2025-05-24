using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Models.NotificationDtos;

public class BaseNotificationDto
{
    public FileDto? File { get; set; }

    public string Text { get; set; }
    public DateTime? WillDoAt { get; set; }
    public TaskPriority Priority { get; set; }
}