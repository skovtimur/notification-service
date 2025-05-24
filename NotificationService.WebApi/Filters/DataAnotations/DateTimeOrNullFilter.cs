using System.ComponentModel.DataAnnotations;

namespace NotificationService.WebApi.Filters.DataAnotations;

public class DateTimeOrNullFilter : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var logger = validationContext.GetRequiredService<ILogger<ValidationFilter>>();

        if (value == null)
            return ValidationResult.Success;

        if (value is not DateTime dateTime)
            return new ValidationResult("The Value must be DateTime or null");

        return dateTime.ToUniversalTime() < DateTime.UtcNow
            ? new ValidationResult("The WillDoAt must be null or a date in the future")
            : ValidationResult.Success;
    }
}