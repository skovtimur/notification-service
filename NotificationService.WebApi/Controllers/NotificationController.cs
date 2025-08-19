using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Abstraction;
using NotificationService.Application.BackgroundJobs;
using NotificationService.Application.BackgroundJobs.Listeners;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Exceptions;
using NotificationService.Domain.Extensions;
using NotificationService.Domain.Models.NotificationDtos;
using NotificationService.Domain.ValueObjects;
using NotificationService.WebApi.Extensions;
using NotificationService.WebApi.Filters;
using NotificationService.WebApi.Queries;
using Quartz;
using Quartz.Impl.Matchers;
using IScheduler = Quartz.IScheduler;

namespace NotificationService.WebApi.Controllers;

[ApiController, Route("api/notify")]
public class NotificationController(
    ILogger<NotificationController> logger,
    ISchedulerFactory schedulerFactory,
    ITaskService taskService,
    IServiceProvider serviceProvider,
    IEmailSender emailSender,
    ITelegramSender telegramSender) : ControllerBase
{
    [HttpPost, Route("by-email"), ValidationFilter]
    public async Task<IActionResult> NotifyByEmail([FromForm, Required] EmailNotifyQuery query)
    {
        if (string.IsNullOrEmpty(query.FromEmailAddress) == false &&
            string.IsNullOrEmpty(query.EmailPassword) == false)
        {
            var exists = await emailSender.Exists(query.FromEmailAddress, query.EmailPassword);
            
            if (!exists)
                return BadRequest("The Email doesn't exist");
        }
        var emailQuery = await query.ToEmailNotificationDto();

        var result = await Notify<EmailNotificationDto, EmailSenderBackgroundJob, EmailSenderListener>(
            query: emailQuery,
            keyOfQuery: "email-query",
            addresses: emailQuery.Emails,
            addressType: AddressType.Email);

        return result;
    }

    [HttpPost, Route("by-telegram"), ValidationFilter]
    public async Task<IActionResult> NotifyByTelegram([FromForm, Required] TelegramNotifyQuery query)
    {
        if (string.IsNullOrEmpty(query.BotApiKey) == false)
        {
            var exists = await telegramSender.Exists(query.BotApiKey);
            
            if (!exists)
                return BadRequest("The TelegramBot doesn't exist");
        }
        var chatIds = new List<TelegramIdValueObject>();

        foreach (var c in query.ChatIds)
        {
            var chatId = TelegramIdValueObject.Create(c);

            if (chatId == null)
                return BadRequest("Chat ID is invalid");

            chatIds.Add(chatId);
        }

        var tgQuery = await query.ToTelegramNotificationDto(chatIds);

        var result = await Notify<TelegramNotificationDto, TelegramSenderBackgroundJob, TelegramSenderListener>(
            query: tgQuery,
            keyOfQuery: "tg-query",
            addresses: tgQuery.ChatIdValueObjects.ToStringList(),
            addressType: AddressType.Telegram);

        return result;
    }

    private async Task<IActionResult> Notify<TQuery, TJob, TListener>(TQuery query, string keyOfQuery,
        List<string> addresses, AddressType addressType)
        where TQuery : BaseNotificationDto
        where TJob : IJob
        where TListener : IJobCustomListener
    {
        var jsonContent = ContentValueObject.Create(query.Text, query.File);

        if (jsonContent == null)
            return BadRequest("A Content is empty. The Contant contain a file or text");

        var newTask = TaskEntity.Create(jsonContent, query.Priority, query.WillDoAt);

        if (newTask == null)
            return BadRequest("The new Task has not been created, it's invalid");

        var recipients = new List<RecipientEntity>();
        foreach (var a in addresses)
        {
            var newRecipient = RecipientEntity.Create(a, addressType, newTask.Id);

            if (newRecipient is null)
                throw new UserCanSendInvalidData($"The Recipient-{a} is not valid");

            recipients.Add(newRecipient);
        }

        // Create a new Task with its Recipients
        await taskService.Add(newTask, recipients);

        //Create a new Job
        await CreateNewJob<TQuery, TJob, TListener>(query, keyOfQuery, newTask, recipients);

        // Send Result
        logger.LogInformation("The New Task-{x} has been created", newTask.Id);
        return Ok(newTask.Id.ToString());
    }

    private async Task CreateNewJob<TQuery, TJob, TListener>(TQuery query, string keyOfQuery, TaskEntity newTask,
        List<RecipientEntity> recipients)
        where TQuery : BaseNotificationDto
        where TJob : IJob
        where TListener : IJobCustomListener
    {
        var listener = serviceProvider.GetRequiredService<TListener>();
        listener.SetName($"job-listener-{newTask.Id}");

        var isWithoutWaiting = query.WillDoAt == null;
        IScheduler scheduler = await schedulerFactory.GetScheduler();

        var jobData = new JobDataMap
        {
            {
                "task-id", newTask.Id
            },
            {
                "recipients", recipients
            },
            {
                keyOfQuery, query
            }
        };
        var jobKey = new JobKey($"job-{newTask.Id}", $"group-{newTask.Id}");
        IJobDetail job = JobBuilder.Create<TJob>()
            .UsingJobData(jobData)
            .WithIdentity(jobKey)
            .Build();

        var triggerBuilder = TriggerBuilder.Create()
            .WithIdentity($"trigger-{newTask.Id}", $"group-{newTask.Id}")
            .ForJob(job)
            .WithPriority((int)newTask.Priority);

        ITrigger trigger = isWithoutWaiting
            ? triggerBuilder.StartNow().Build()
            : triggerBuilder.StartAt(startTimeUtc: (DateTime)query.WillDoAt!).Build();

        scheduler.ListenerManager.AddJobListener(listener,
            KeyMatcher<JobKey>
                .KeyEquals(new JobKey($"job-{newTask.Id}", $"group-{newTask.Id}")));

        await scheduler.ScheduleJob(job, trigger);
    }
}