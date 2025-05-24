using Microsoft.Extensions.Logging;
using NotificationService.Application.Abstraction;
using NotificationService.Application.Extensions;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Models;
using NotificationService.Domain.Models.NotificationDtos;
using Quartz;

namespace NotificationService.Application.BackgroundJobs;

public class TelegramSenderBackgroundJob(
    ITelegramSender telegramSender,
    ITaskStatusService taskStatusService,
    IRecipientService recipientService,
    ILogger<TelegramSenderBackgroundJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;

        var taskId = dataMap.GetValueOrThrowException<long>("task-id");
        var recipients = dataMap.GetValueOrThrowException<List<RecipientEntity>>("recipients");
        var query = dataMap.GetValueOrThrowException<TelegramNotificationDto>("tg-query");

        taskStatusService.GetToWork(taskId);

        foreach (var recipient in recipients)
        {
            try
            {
                await telegramSender.SendMessage(recipient.Address, query.Text, query.File, query.BotApiKey);
                await recipientService.HasBeenSuccessCompleted(recipient.Id);
            }
            catch (Exception e)
            {
                var errorMessage = $"{e.GetType()}: {e.Message}";

                await recipientService.HasBeenGottenErrors(recipient.Id, errorMessage);
                logger.LogDebug(e, "Failed to send telegram({x}) notification. {e}",
                    recipient.Address, errorMessage);
            }
        }
        logger.LogTrace($"The {nameof(TelegramSenderBackgroundJob)} was executed, task id: {taskId}");
        await taskStatusService.HasBeenCompleted(taskId);
    }
}