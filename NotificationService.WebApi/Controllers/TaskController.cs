using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Abstraction;
using NotificationService.Domain.Enums;
using NotificationService.WebApi.Filters;
using NotificationService.WebApi.Queries;
using Quartz;

namespace NotificationService.WebApi.Controllers;

[ApiController, Route("api/tasks")]
public class TaskController(
    ITaskService taskService,
    ITaskStatusService taskStatusService,
    ILogger<TaskController> logger,
    ISchedulerFactory factory) : ControllerBase
{
    public const string TaskQuantityHeader = "X-Task-Quantity";

    [HttpGet, Route("{taskId:long}"), ValidationFilter]
    public async Task<IActionResult> Get([Required] long taskId)
    {
        var foundTask = await taskService.Get(taskId);

        if (foundTask == null)
            return NotFound("The Task not found");

        return Ok(foundTask);
    }

    [HttpGet, Route("history"), ValidationFilter]
    public async Task<IActionResult> GetHistory([Required, FromQuery] GetHistoryQuery query)
    {
        if (query.InformationVolume == TaskInformationVolume.Full)
        {
            var dtos =
                await taskService.GetHistory(query.UtcTimeFrom.ToUniversalTime(),
                    query.DemonstrationMethod, query.SortingType);

            Response.Headers.Append(TaskQuantityHeader, dtos.Count().ToString());
            return Ok(dtos);
        }

        var partialDtos =
            await taskService.GetPartialHistory(query.UtcTimeFrom.ToUniversalTime(),
                query.DemonstrationMethod, query.SortingType);

        Response.Headers.Append(TaskQuantityHeader, partialDtos.Count().ToString());
        return Ok(partialDtos);
    }


    [HttpPatch, Route("cancel/{taskId:long}"), ValidationFilter]
    public async Task<IActionResult> CancelTask([Required] long taskId)
    {
        var taskExist = await taskService.Exists(taskId);

        if (taskExist == false)
            return NotFound("The Task not found");

        await taskStatusService.HasBeenCancelled(taskId);

        var scheduler = await factory.GetScheduler();

        await InterruptOrPauseJob(scheduler, $"Task {taskId} has been cancelled", taskId,
            TaskBanType.Cancelled);

        return Ok();
    }

    [HttpDelete, Route("remove/{taskId:long}"), ValidationFilter]
    public async Task<IActionResult> RemoveTask([Required] long taskId)
    {
        var taskExist = await taskService.Remove(taskId);

        if (taskExist == false)
            return NotFound("The Task not found");

        var scheduler = await factory.GetScheduler();

        await InterruptOrPauseJob(scheduler, $"The task {taskId} has been removed", taskId,
            TaskBanType.Deleted);

        return Ok();
    }

    private async Task InterruptOrPauseJob(IScheduler scheduler, string logText, long taskId,
        TaskBanType banType)
    {
        var key = new JobKey($"job-{taskId}", $"group-{taskId}");
        var foundJob = await scheduler.GetJobDetail(key);

        if (foundJob == null)
        {
            logger.LogWarning($"The Job-{taskId} has not been found");
            return;
        }
        foundJob.JobDataMap["ban-type"] = banType;

        var interrupted = await scheduler.Interrupt(key);
        
        if (!interrupted)
            await scheduler.PauseJob(key);
        
        logger.LogInformation(logText);
    }
}