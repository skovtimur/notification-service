using System.ComponentModel.DataAnnotations;
using NotificationService.WebApi.Queries;

namespace NotificationService.WebApi.Filters.DataAnotations;

public class RequiredTextOrFileFilter : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var logger = validationContext.GetRequiredService<ILogger<ValidationFilter>>();

        if (value is not BaseNotifyQuery query)
            return new ValidationResult("The Value must be of type BaseNotifyQuery");

        return query.File == null && string.IsNullOrEmpty(query.Text)
            ? new ValidationResult("The BaseNotifyQuery must contain at least a file or a text")
            : ValidationResult.Success;
    }
}