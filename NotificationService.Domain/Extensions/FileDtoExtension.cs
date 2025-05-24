using NotificationService.Domain.Models;

namespace NotificationService.Domain.Extensions;

public static class FileDtoExtension
{
    public static bool IsImage(this FileDto file) => ImageAllowedMimeTypes.Contains(file.MimeType);
    public static bool IsVideo(this FileDto file) => VideoAllowedMimeTypes.Contains(file.MimeType);
    public static bool IsAudio(this FileDto file) => AudioAllowedMimeTypes.Contains(file.MimeType);
    public static bool IsDocument(this FileDto file) => DocumentAllowedMimeTypes.Contains(file.MimeType);


    public static FileType? GetFileType(this FileDto file)
    {
        if (file.IsImage()) return FileType.Image;
        if (file.IsAudio()) return FileType.Audio;
        if (file.IsVideo()) return FileType.Video;
        if (file.IsDocument()) return FileType.Document;

        return null;
    }

    public static readonly List<string> ImageAllowedMimeTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/avif",
        "image/gif"
    ];

    public static readonly List<string> VideoAllowedMimeTypes =
    [
        "video/mp4",
    ];

    public static readonly List<string> AudioAllowedMimeTypes =
    [
        "audio/x-aiff",
        "audio/mpeg",
        "audio/basic"
    ];

    public static readonly List<string> DocumentAllowedMimeTypes =
    [
        "text/plain",
        "application/msword",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "application/pdf",
        "application/x-pdf",
    ];
}