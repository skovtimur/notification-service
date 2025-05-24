using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using NotificationService.Domain.Validators;

namespace NotificationService.WebApi.Filters.DataAnotations;

public class EmailOrNullFilter : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        var regex = new Regex(EmailValidator.EmailPattern);

        return value is string address && regex.IsMatch(address)
            ? ValidationResult.Success
            : new ValidationResult("The Address isn't valid");
    }
}