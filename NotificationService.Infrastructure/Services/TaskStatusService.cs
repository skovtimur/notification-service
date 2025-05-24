using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Abstraction;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Exceptions;
using NotificationService.Infrastructure.Database;
using TaskStatus = NotificationService.Domain.Entities.TaskStatus;

namespace NotificationService.Infrastructure.Services;

public class TaskStatusService(
    ILogger<TaskStatusService> logger,
    IDbContextFactory<MainContext> dbContextFactory) : ITaskStatusService
{
    // Change Status Methods:
    public void GetToWork(long id)
    {
        //Данный метод НЕ асинх. потому что если запрос смены статуса на Working придет позже чем другие запросы на измен. статуса вроде(HasBeenSuccesfullyWorked or HasBeenCancelled),
        //тогда они не смогут найти нужный таск который на тот момент будет иметь статус Working(это важно чтобы они не имели возможность менять ошибочно статус выполненным задачам)
        using var dbContext = dbContextFactory.CreateDbContext();
        var foundTask = dbContext.Tasks.FirstOrDefault(x => x.Id == id);

        if (foundTask == null)
        {
            logger.LogCritical("The Task Service has not found the task-{x}", id);
            throw new TaskStatusWasNotUpdatedException(id);
        }

        foundTask.Status = TaskStatus.Working;
        dbContext.SaveChanges();
    }

    public async Task HasBeenCompleted(long id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var foundTask = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id
                                                                       && x.Status == TaskStatus.Working);

        if (foundTask == null)
        {
            logger.LogCritical("The Task Service has not found the task-{x}", id);
            throw new TaskStatusWasNotUpdatedException(id);
        }

        foundTask.Status = TaskStatus.Completed;
        foundTask.CompletedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task HasBeenCancelled(long id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var foundTask = await dbContext.Tasks
            .FirstOrDefaultAsync(x => x.Id == id
                                      && (x.Status == TaskStatus.Working ||
                                          x.Status == TaskStatus.Waiting));

        if (foundTask == null)
        {
            logger.LogCritical("The Task Service has not found the task-{x}", id);
            throw new TaskStatusWasNotUpdatedException(id);
        }

        foundTask.Status = TaskStatus.Cancelled;
        await dbContext.SaveChangesAsync();
    }
}