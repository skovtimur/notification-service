using System.Text.RegularExpressions;
using FluentValidation;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Models;

namespace NotificationService.Domain.Validators;

public class FileValidator : AbstractValidator<FileDto>
{
    public FileValidator()
    {
        RuleFor(x => x.FileName).NotEmpty().WithMessage("File name cannot be empty");
        RuleFor(x => x.FileContent).Must(x => x.Length > 0).WithMessage("File content cannot be empty");

        RuleFor(x => x.Weigh).Must(x => x > 0).WithMessage("Weigh cannot be empty");
        RuleFor(x => x).Must(x => x.Weigh == x.FileContent.Length)
            .WithMessage("File content length must be equal to file content");
        RuleFor(x => x.MimeType).NotEmpty().Must(x => Regex.IsMatch(x, MimeTypePattern))
            .WithMessage("File mime type cannot be empty");
    }

    private const string MimeTypePattern = @"\w+/[-.\w]+(?:\+[-.\w]+)?";
    public static bool IsValid(FileDto file) => new FileValidator().Validate(file).IsValid;
}