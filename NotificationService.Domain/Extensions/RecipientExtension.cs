using NotificationService.Domain.Entities;
using NotificationService.Domain.Models;

namespace NotificationService.Domain.Extensions;

public static class RecipientExtension
{
    public static RecipientDto ToDto(this RecipientEntity recipient)
    {
        return new RecipientDto
        {
            Address = recipient.Address,
            AddressType = recipient.AddressType.ToString(),
            SendingStatus = recipient.SendingStatus,
            ErrorText = recipient.ErrorText,
            CompletedAt = recipient.CompletedAt,
        };
    }

    public static IEnumerable<RecipientDto> ToDtos(this IEnumerable<RecipientEntity> list)
    {
        return list.Select(ToDto);
    }
}