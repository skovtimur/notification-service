namespace NotificationService.Domain.Exceptions;

public class NoImportantDataException(string message) : Exception, ICriticalException
{
    public override string Message { get; } = message;
}