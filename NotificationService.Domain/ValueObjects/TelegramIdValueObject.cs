using NotificationService.Domain.Validators;

namespace NotificationService.Domain.ValueObjects;

public class TelegramIdValueObject
{
    public long ChatId { get; set; }

    public static TelegramIdValueObject? Create(long chatId)
    {
        var id = new TelegramIdValueObject { ChatId = chatId };

        return TelegramIdValidator.IsValid(id)
            ? id
            : null;
    }
}