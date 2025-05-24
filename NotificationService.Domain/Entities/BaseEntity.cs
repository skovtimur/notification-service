namespace NotificationService.Domain.Entities;

public class BaseEntity
{
    public long Id { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public BaseEntity()
    {
        Id = new Random().NextInt64(1, long.MaxValue);
    }
}