using System.ComponentModel.DataAnnotations;
using NotificationService.WebApi.Filters.DataAnotations;

namespace NotificationService.WebApi.Queries;

public class EmailNotifyQuery : BaseNotifyQuery
{
    [Required, EmailAdressesFilter] public required List<string> Emails { get; set; }

    [EmailOrNullFilter] public string? FromEmailAddress { get; set; }
    public string? EmailPassword { get; set; }
}