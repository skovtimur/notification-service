using NotificationService.Domain.Models;

namespace NotificationService.Application.Abstraction;

public interface ITelegramSender
{
    public Task SendMessage(
        string chatId,
        string text,
        FileDto file,
        string? apiKey = null);

    public Task<bool> Exists(string botApiKey);
}