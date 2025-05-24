using FluentValidation;
using NotificationService.Domain.Models;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Validators;

public class
    JsonContentDtoValidator : AbstractValidator<JsonContentValueObject>
{
    public JsonContentDtoValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }

    public static bool IsValid(JsonContentValueObject jsonContent) =>
        new JsonContentDtoValidator().Validate(jsonContent).IsValid;
}