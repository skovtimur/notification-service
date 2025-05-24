using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using NotificationService.Domain.Validators;

namespace NotificationService.WebApi.Filters.DataAnotations;

public class EmailAdressesFilter : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not List<string> addresses)
            return new ValidationResult("The Value isn't List<string>");

        var regex = new Regex(EmailValidator.EmailPattern);

        return addresses.Any(regex.IsMatch) == false
            ? new ValidationResult("The Address isn't valid")
            : ValidationResult.Success;
    }
}