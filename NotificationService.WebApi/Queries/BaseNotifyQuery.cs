using System.ComponentModel.DataAnnotations;
using NotificationService.Domain.Entities;
using NotificationService.WebApi.Filters.DataAnotations;

namespace NotificationService.WebApi.Queries;

[RequiredTextOrFileFilter]
public class BaseNotifyQuery
{
    [AllowedMimeFileTypeFilter] public IFormFile? File { get; set; }

    [Required, StringLength(5000, MinimumLength = 25)]
    public required string Text { get; set; }

    [Required] public required TaskPriority Priority { get; set; } = TaskPriority.NoMatter;

    [DateTimeOrNullFilter] public DateTime? WillDoAt { get; set; }
}