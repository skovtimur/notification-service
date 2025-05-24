namespace NotificationService.Domain.Models;

public class TaskDto
{
    public long Id { get; set; }

    public string JsonContent { get; set; }
    public string Priority { get; set; }

    public IEnumerable<RecipientDto> RecipientsDtos { get; set; }


    public bool IsDeleted { get; set; }
    public string? ErrorText { get; set; }
    public string Status { get; set; }

    
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? MustBeginAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}