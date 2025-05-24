using FluentValidation;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Exceptions;

namespace NotificationService.Domain.Validators;

public class RecipientValidator : AbstractValidator<RecipientEntity>
{
    public RecipientValidator()
    {
        RuleFor(x => x.Address).NotNull().NotEmpty();
        RuleFor(x => x.AddressType).NotNull();
        RuleFor(x => x.TaskId).NotEmpty();
        
        RuleFor(x => x).Must(IsAddressAllowed);
    }

    private static readonly EmailValidator EmailValidator = new();
    private static readonly TelegramIdValidator TelegramIdValidator = new();

    private static bool IsAddressAllowed(RecipientEntity address)
    {
        switch (address.AddressType)
        {
            case AddressType.Telegram:
                return TelegramIdValidator.Validate(address.Address);

            case AddressType.Email:
                return EmailValidator.Validate(address.Address);

            default:
                throw new UnsupportedTypeException("The value doesn't match the required format");
        }
    }

    public static bool IsValid(RecipientEntity address) =>
        new RecipientValidator().Validate(address).IsValid;
}