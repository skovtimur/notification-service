using Microsoft.Extensions.Logging;
using NotificationService.Application.Abstraction;
using NotificationService.Application.Extensions;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Models.NotificationDtos;
using Quartz;

namespace NotificationService.Application.BackgroundJobs;

public class EmailSenderBackgroundJob(
    IEmailSender emailSender,
    ITaskStatusService taskStatusService,
    IRecipientService recipientService,
    ILogger<EmailSenderBackgroundJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;

        var taskId = dataMap.GetValueOrThrowException<long>("task-id");
        var recipients = dataMap.GetValueOrThrowException<List<RecipientEntity>>("recipients");
        var emailQuery = dataMap.GetValueOrThrowException<EmailNotificationDto>("email-query");

        taskStatusService.GetToWork(taskId);

        foreach (var recipient in recipients)
        {
            try
            {
                await emailSender.SendMessage(recipient.Address, emailQuery.Text, emailQuery.File,
                    emailQuery.FromEmailAddress, emailQuery.EmailPassword);
                
                await recipientService.HasBeenSuccessCompleted(recipient.Id);
            }
            catch (Exception e)
            {
                var errorMessage = $"{e.GetType()}: {e.Message}";

                await recipientService.HasBeenGottenErrors(recipient.Id, errorMessage);
                logger.LogInformation(e, "Failed to send email({x}) notification. {e}",
                    recipient.Address, errorMessage);
            }
        }

        logger.LogTrace($"The {nameof(EmailSenderBackgroundJob)} was executed, task id: {taskId}");
        await taskStatusService.HasBeenCompleted(taskId);
    }
}