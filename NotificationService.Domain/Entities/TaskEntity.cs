using NotificationService.Domain.Validators;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Entities;

public class TaskEntity : BaseEntity
{
    // Content
    public ContentValueObject Content { get; set; }

    //Priority
    public TaskPriority Priority { get; set; }

    // Recipients
    public List<RecipientEntity> Recipients { get; set; }


    // Status
    public TaskStatus Status { get; set; }

    // Dates
    public DateTime? MustBeginAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public static TaskEntity? Create(ContentValueObject content, TaskPriority priority, DateTime? mustBeginAt = null)
    {
        var newTaskEntity = new TaskEntity
        {
            Content = content,
            Priority = priority,
            MustBeginAt = mustBeginAt,
            Status = TaskStatus.Waiting
        };

        return TaskEntityValidator.IsValid(newTaskEntity)
            ? newTaskEntity
            : null;
    }
}

public enum TaskPriority
{
    NoMatter,
    Low,
    Medium,
    High,
    Immediately
}

public enum TaskStatus
{
    Waiting,
    Working,
    Cancelled,
    Completed
}