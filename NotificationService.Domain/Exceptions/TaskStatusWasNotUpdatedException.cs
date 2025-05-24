namespace NotificationService.Domain.Exceptions;

public class TaskStatusWasNotUpdatedException(long taskId) : Exception, ICriticalException
{
    public override string Message { get; } = $"The Task-{taskId} has not been updated. The Task Service couldn't found it";
}