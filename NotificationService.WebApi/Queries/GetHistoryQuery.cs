using System.ComponentModel.DataAnnotations;
using NotificationService.Domain.Enums;

namespace NotificationService.WebApi.Queries;

public class GetHistoryQuery
{
    [Required] public required DateTime UtcTimeFrom { get; set; }

    public required TasksDemonstrationMethod DemonstrationMethod { get; set; } =
        TasksDemonstrationMethod.OnlyNotDeleted;

    public required TaskInformationVolume InformationVolume { get; set; } = TaskInformationVolume.Full;
    public required TaskSortingType SortingType { get; set; } = TaskSortingType.DescendingDate;
}