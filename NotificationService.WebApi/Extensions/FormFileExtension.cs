using NotificationService.Domain.Exceptions;
using NotificationService.Domain.Extensions;
using NotificationService.Domain.Models;

namespace NotificationService.WebApi.Extensions;

public static class FormFileExtension
{
    public static bool IsImage(this IFormFile file) =>
        FileDtoExtension.ImageAllowedMimeTypes.Contains(file.ContentType);

    public static bool IsVideo(this IFormFile file) =>
        FileDtoExtension.VideoAllowedMimeTypes.Contains(file.ContentType);

    public static bool IsAudio(this IFormFile file) =>
        FileDtoExtension.AudioAllowedMimeTypes.Contains(file.ContentType);

    public static bool IsDocument(this IFormFile file) =>
        FileDtoExtension.DocumentAllowedMimeTypes.Contains(file.ContentType);

    public static async Task<FileDto> ToFileDto(this IFormFile file)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        stream.Position = 0;
        var dto = FileDto.Create(file.FileName, stream.ToArray(), file.ContentType, stream.Length);

        if (dto is null)
            throw new IsNullOrEmptyException("FileDto is null");

        return dto;
    }
}