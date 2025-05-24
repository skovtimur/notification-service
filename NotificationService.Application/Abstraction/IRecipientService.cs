using NotificationService.Domain.Entities;

namespace NotificationService.Application.Abstraction;

public interface IRecipientService
{
    public Task AddRecipient(RecipientEntity newRecipient);
    public Task HasBeenGottenErrors(long id, string errorText);
    public Task HasBeenSuccessCompleted(long id);
}