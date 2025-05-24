namespace NotificationService.Application.Abstraction;

public interface ITaskStatusService
{
    public void GetToWork(long id);
    public Task HasBeenCompleted(long taskId);
    public Task HasBeenCancelled(long taskId);
}