using System.ComponentModel.DataAnnotations;
using NotificationService.Domain.Entities;
using NotificationService.WebApi.Filters;
using NotificationService.WebApi.Filters.DataAnotations;

namespace NotificationService.WebApi.Queries;

[RequiredTextOrFileFilter]
public class BaseNotifyQuery
{
    [AllowedMimeFileTypeFilter] public IFormFile? File { get; set; }

    [Required, StringLength(5000, MinimumLength = 25)] public string Text { get; set; }
    
    [Required] public TaskPriority Priority { get; set; }
    
    [DateTimeOrNullFilter] public DateTime? WillDoAt { get; set; }
}