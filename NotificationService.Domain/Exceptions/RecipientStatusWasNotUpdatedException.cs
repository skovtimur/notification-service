namespace NotificationService.Domain.Exceptions;

public class RecipientStatusWasNotUpdatedException(long taskId) : Exception, ICriticalException
{
    public override string Message { get; } =
        $"The Recipient-{taskId} has not been updated. The Recipient Service couldn't found it";
}