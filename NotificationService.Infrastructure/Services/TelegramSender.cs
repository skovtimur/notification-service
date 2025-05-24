using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Abstraction;
using NotificationService.Domain.Exceptions;
using NotificationService.Domain.Models;
using Telegram.Bot;
using FileType = NotificationService.Domain.Models.FileType;

namespace NotificationService.Infrastructure.Services;

public class TelegramSender : ITelegramSender
{
    private readonly TelegramBotClient _defaultBotClient;
    private readonly ILogger<TelegramSender> _logger;

    public TelegramSender(IConfiguration configuration, ILogger<TelegramSender> logger)
    {
        _logger = logger;
        var apiKey = configuration.GetValue<string>("UserSecrets:ApiKeyOfTelegramBot");

        if (string.IsNullOrEmpty(apiKey))
            throw new IsNullOrEmptyException("UserSecrets is empty");

        _defaultBotClient = new TelegramBotClient(apiKey);
    }

    public async Task SendMessage(string chatId, string text, FileDto file, string? apiKey = null)
    {
        var client = string.IsNullOrEmpty(apiKey)
            ? _defaultBotClient
            : new TelegramBotClient(apiKey);

        if (chatId is long)
            throw new UserCanSendInvalidData("ChatId isn't long");

        if (file == null)
        {
            await client.SendMessage(chatId, text);
            return;
        }

        var stream = new MemoryStream(file.FileContent);

        switch (file.FileType)
        {
            case FileType.Image:
                await client.SendPhoto(chatId, stream,
                    $"{text ?? ""}");
                break;

            case FileType.Audio:
                await client.SendAudio(chatId, stream,
                    $"{text ?? ""}");
                break;

            case FileType.Video:
                await client.SendVideo(chatId, stream,
                    $"{text ?? ""}");
                break;

            case FileType.Document:
                await client.SendDocument(chatId, stream,
                    $"{text ?? ""}");
                break;
        }
    }

    public async Task<bool> Exists(string botApiKey)
    {
        try
        {
            var client = new TelegramBotClient(botApiKey);
            var info = await client.GetMe();
            
            return info != null && info.IsBot;
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.Message);
            return false;
        }
    }
}