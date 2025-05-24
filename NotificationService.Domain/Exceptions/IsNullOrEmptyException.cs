namespace NotificationService.Domain.Exceptions;

public class IsNullOrEmptyException(string message) : Exception(message), ICriticalException;