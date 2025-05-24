using System.Text.Json;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Models;
using NotificationService.Domain.Validators;

namespace NotificationService.Domain.ValueObjects;

public class ContentValueObject
{
    public string JsonContent { get; set; }

    public static ContentValueObject? Create(string? text, FileDto? file)
    {
        var jsonContent = JsonContentValueObject.Create(text, file);
        
        if (jsonContent == null)
            return null;

        var newContent = new ContentValueObject
        {
            JsonContent = JsonSerializer.Serialize(jsonContent)
        };

        return ContentValueObjectValidator.IsValid(newContent)
            ? newContent
            : null;
    }
}