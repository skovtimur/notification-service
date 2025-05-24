using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Extensions;

public static class TelegramIdentificationsExtensions
{
    public static List<string> ToStringList(this List<TelegramIdValueObject> telegramIds) =>
        telegramIds.Select(tgId => tgId.ChatId.ToString()).ToList();
}