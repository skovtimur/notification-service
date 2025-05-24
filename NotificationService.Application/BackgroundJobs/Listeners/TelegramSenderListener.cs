using Microsoft.Extensions.Logging;
using NotificationService.Application.Abstraction;
using NotificationService.Application.Extensions;
using NotificationService.Domain.Enums;
using Quartz;

namespace NotificationService.Application.BackgroundJobs.Listeners;

public class TelegramSenderListener(ILogger<TelegramSenderListener> logger, ITaskStatusService taskStatusService)
    : IJobCustomListener
{
    public Task JobToBeExecuted(IJobExecutionContext context,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.CompletedTask;

        logger.LogInformation($"The TelegramSender job start execution, taskId: {GetTaskId(context)}");
        return Task.CompletedTask;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.CompletedTask;

        var taskId = GetTaskId(context);
        var banType = context.JobDetail.JobDataMap
            .GetValueOrThrowException<TaskBanType>("ban-type");

        //Методы по смене статуса вызваны в action-ах
        switch (banType)
        {
            case TaskBanType.Cancelled:
                logger.LogInformation($"The TelegramSender job has been cancelled, taskId: {taskId}");
                break;
            case TaskBanType.Deleted:
                logger.LogInformation($"The TelegramSender job has been deleted, taskId: {taskId}");
                break;
        }

        return Task.CompletedTask;
    }

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromResult(Task.CompletedTask);

        logger.LogInformation($"The TelegramSender job executed, taskId: {GetTaskId(context)}");
        return Task.FromResult(Task.CompletedTask);
    }

    public string Name => _name;

    public void SetName(string initName)
    {
        if (string.IsNullOrEmpty(_name))
            _name = initName;
    }

    private string _name = string.Empty;

    private static long GetTaskId(IJobExecutionContext context)
    {
        return context.JobDetail.JobDataMap
            .GetValueOrThrowException<long>("task-id");
    }
}