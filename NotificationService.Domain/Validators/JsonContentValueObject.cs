using NotificationService.Domain.Validators;

namespace NotificationService.Domain.Models;

public class JsonContentValueObject
{
    public string Text { get; set; }
    public FileDto File { get; set; }

    public static JsonContentValueObject? Create(string? text, FileDto? file)
    {
        var jsonContent = new JsonContentValueObject { Text = text, File = file };

        return JsonContentDtoValidator.IsValid(jsonContent)
            ? jsonContent
            : null;
    }
}