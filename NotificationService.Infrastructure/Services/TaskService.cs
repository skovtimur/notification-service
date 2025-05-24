using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Abstraction;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Extensions;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Database;

namespace NotificationService.Infrastructure.Services;

public class TaskService(
    ILogger<TaskService> logger,
    MainContext dbContext,
    IMapper mapper) : ITaskService
{
    public async Task Add(TaskEntity newTask, List<RecipientEntity> recipients)
    {
        await dbContext.AddAsync(newTask);
        await dbContext.Recipients.AddRangeAsync(recipients);
        await dbContext.SaveChangesAsync();

        logger.LogTrace($"Has added a task-{newTask.Id} to db");
    }

    public async Task<TaskDto?> Get(long taskId)
    {
        var foundTask = await dbContext.Tasks.Include(x => x.Recipients)
            .FirstOrDefaultAsync(x => x.Id == taskId);

        if (foundTask == null)
            return null;

        var mappedTask = mapper.Map<TaskDto>(foundTask);
        mappedTask.RecipientsDtos = foundTask.Recipients.ToDtos();

        return mappedTask;
    }

    public async Task<bool> Exists(long taskId)
    {
        var exists = await dbContext.Tasks.AnyAsync(x => x.Id == taskId);
        return exists;
    }

    public async Task<IEnumerable<TaskParticleDto>> GetPartialHistory(DateTime utcTimeFrom,
        TasksDemonstrationMethod demonstrationMethod, TaskSortingType sortingType)
    {
        var tasks = await GetHistory<TaskParticleDto>(utcTimeFrom, demonstrationMethod, sortingType);
        return tasks;
    }

    public async Task<IEnumerable<TaskDto>> GetHistory(DateTime utcTimeFrom,
        TasksDemonstrationMethod demonstrationMethod, TaskSortingType sortingType)
    {
        var tasks = await GetHistory<TaskDto>(utcTimeFrom, demonstrationMethod, sortingType);
        return tasks;
    }

    private async Task<IEnumerable<TaskDtoT>> GetHistory<TaskDtoT>(DateTime utcTimeFrom,
        TasksDemonstrationMethod demonstrationMethod, TaskSortingType sortingType)
    {
        var query = dbContext.Tasks
            .Include(x => x.Recipients)
            .IgnoreQueryFilters()
            .Where(x => x.CreatedAt > utcTimeFrom);

        query = demonstrationMethod switch
        {
            TasksDemonstrationMethod.OnlyNotDeleted => query.Where(x => x.IsDeleted == false),
            TasksDemonstrationMethod.OnlyDeleted => query.Where(x => x.IsDeleted == true),
            _ => query
        };

        query = sortingType == TaskSortingType.AscedingDate
            ? query.OrderBy(x => x.CreatedAt)
            : query.OrderByDescending(x => x.CreatedAt);
        ;

        var tasks = await query
            .Select(x => mapper.Map<TaskDtoT>(x)).ToListAsync();

        return tasks;
    }

    public async Task<bool> Remove(long taskId)
    {
        var removedTask = await dbContext.Tasks
            .Include(x => x.Recipients)
            .FirstOrDefaultAsync(x => x.Id == taskId);

        if (removedTask == null)
            return false;

        foreach (var r in removedTask.Recipients)
        {
            r.IsDeleted = true;
            r.DeletedAt = DateTime.UtcNow;
        }

        removedTask.IsDeleted = true;
        removedTask.DeletedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        logger.LogTrace($"The task-{taskId} has been removed from db");
        return true;
    }
}