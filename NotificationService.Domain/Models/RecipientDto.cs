using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Models;

public class RecipientDto
{
    public string Address { get; set; }
    public string AddressType { get; set; }
    
    public SendingStatus SendingStatus { get; set; }
    public string? ErrorText { get; set; }
    public DateTime? CompletedAt { get; set; }
}