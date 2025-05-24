using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Validators;

public class TelegramIdValidator : IValidator<string>
{
    public bool Validate(string telegramIdString) => long.TryParse(telegramIdString, out var i);


    public static bool IsValid(TelegramIdValueObject telegramId) => new TelegramIdValidator()
        .Validate(telegramId.ChatId.ToString());
}