using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Abstraction;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Exceptions;
using NotificationService.Infrastructure.Database;

namespace NotificationService.Infrastructure.Services;

public class RecipientService(
    ILogger<RecipientService> logger,
    MainContext context) : IRecipientService
{
    public async Task AddRecipient(RecipientEntity newRecipient)
    {
        await context.Recipients.AddAsync(newRecipient);
        await context.SaveChangesAsync();
    }

    public async Task HasBeenGottenErrors(long id, string errorText)
    {
        var foundRecipient = await context.Recipients
            .FirstOrDefaultAsync(x => x.Id == id
                                      && x.SendingStatus ==
                                      SendingStatus.None);

        if (foundRecipient == null)
        {
            logger.LogCritical("The Recipient Service has not found the recipient-{x}", id);
            throw new RecipientStatusWasNotUpdatedException(id);
        }

        foundRecipient.SendingStatus = SendingStatus.CompletedWithError;
        foundRecipient.CompletedAt = DateTime.UtcNow;
        foundRecipient.ErrorText = errorText;

        await context.SaveChangesAsync();
    }

    public async Task HasBeenSuccessCompleted(long id)
    {
        var foundRecipient = await context.Recipients
            .FirstOrDefaultAsync(x => x.Id == id
                                      && x.SendingStatus ==
                                      SendingStatus.None);

        if (foundRecipient == null)
        {
            logger.LogCritical("The Task Service has not found the task-{x}", id);
            throw new RecipientStatusWasNotUpdatedException(id);
        }

        foundRecipient.SendingStatus = SendingStatus.SuccessCompleted;
        foundRecipient.CompletedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }
}