namespace NotificationService.Domain.Exceptions;

public class UserCanSendInvalidData(string message) : Exception(message), ICriticalException;