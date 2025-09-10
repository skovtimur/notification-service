using System.ComponentModel.DataAnnotations;
using NotificationService.WebApi.Extensions;

namespace NotificationService.WebApi.Filters.DataAnotations;

public class AllowedMimeFileTypeFilter : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        var logger = validationContext.GetRequiredService<ILogger<ValidationFilter>>();

        if (value is IFormFile formFile == false)
        {
            const string errorText = "The object isn't FileModel type";

            logger.LogTrace(errorText);
            return new ValidationResult(errorText);
        }

        if (formFile.IsImage()
            || formFile.IsVideo()
            || formFile.IsAudio()
            || formFile.IsDocument())
            return ValidationResult.Success;

        const string notSupportedText = "The File isn't an image, video, audio or document. His type isn't supported.";

        logger.LogTrace(notSupportedText);
        return new ValidationResult(notSupportedText);
    }
}