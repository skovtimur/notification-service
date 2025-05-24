using System.ComponentModel.DataAnnotations;
using NotificationService.Domain.Enums;

namespace NotificationService.WebApi.Queries;

public class GetHistoryQuery
{
    [Required] public DateTime UtcTimeFrom { get; set; }
    public TasksDemonstrationMethod DemonstrationMethod { get; set; } = TasksDemonstrationMethod.OnlyNotDeleted;
    public TaskInformationVolume InformationVolume { get; set; } = TaskInformationVolume.Full;
    public TaskSortingType SortingType { get; set; } = TaskSortingType.DescendingDate;
}