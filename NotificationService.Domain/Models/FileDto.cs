using NotificationService.Domain.Exceptions;
using NotificationService.Domain.Extensions;
using NotificationService.Domain.Validators;

namespace NotificationService.Domain.Models;

public class FileDto
{
    public string FileName { get; set; }
    public string MimeType { get; set; }
    public FileType FileType { get; set; }
    public byte[] FileContent { get; set; }
    public long Weigh { get; set; }

    public static FileDto? Create(string fileName, byte[] fileContent, string mimeType, long weigh)
    {
        var newFile = new FileDto
        {
            FileName = fileName,
            MimeType = mimeType,
            FileContent = fileContent,
            Weigh = weigh
        };
        var fileType = newFile.GetFileType();

        if (fileType == null)
            throw new UnsupportedTypeException("The File type is unsupported");

        newFile.FileType = (FileType)fileType;
        return FileValidator.IsValid(newFile)
            ? newFile
            : null;
    }
}

public enum FileType
{
    Image,
    Audio,
    Video,
    Document,
}