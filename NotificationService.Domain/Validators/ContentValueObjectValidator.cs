using FluentValidation;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Validators;

public class ContentValueObjectValidator : AbstractValidator<ContentValueObject>
{
    public ContentValueObjectValidator()
    {
        RuleFor(x => x.JsonContent).NotEmpty()
            .WithMessage("JsonContent is required.");
    }

    public static bool IsValid(ContentValueObject contentValueObject) =>
        new ContentValueObjectValidator().Validate(contentValueObject).IsValid;
}