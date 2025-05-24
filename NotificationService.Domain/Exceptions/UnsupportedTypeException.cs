namespace NotificationService.Domain.Exceptions;

public class UnsupportedTypeException(string message) : Exception(message), ICriticalException;