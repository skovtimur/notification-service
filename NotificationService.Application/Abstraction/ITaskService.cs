using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Abstraction;

public interface ITaskService
{
    public Task Add(TaskEntity newTask, List<RecipientEntity> recipients);
    public Task<bool> Remove(long taskId);


    public Task<TaskDto?> Get(long taskId);
    public Task<bool> Exists(long taskId);

    public Task<IEnumerable<TaskDto>> GetHistory(DateTime utcTimeFrom, TasksDemonstrationMethod demonstrationMethod,
        TaskSortingType sortingType);

    public Task<IEnumerable<TaskParticleDto>> GetPartialHistory(DateTime utcTimeFrom,
        TasksDemonstrationMethod demonstrationMethod, TaskSortingType sortingType);
}