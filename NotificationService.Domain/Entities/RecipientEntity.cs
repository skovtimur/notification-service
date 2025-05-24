using NotificationService.Domain.Validators;

namespace NotificationService.Domain.Entities;

public class RecipientEntity : BaseEntity
{
    public string Address { get; set; }
    public AddressType AddressType { get; set; }

    public TaskEntity Task { get; set; }
    public long TaskId { get; set; }
    
    public SendingStatus SendingStatus { get; set; }
    public string? ErrorText { get; set; }
    public DateTime? CompletedAt { get; set; }


    public static RecipientEntity? Create(string address, AddressType addressType, long taskId)
    {
        var newRecipient = new RecipientEntity
        {
            Address = address,
            AddressType = addressType,
            TaskId = taskId,
            SendingStatus = SendingStatus.None
        };

        return RecipientValidator.IsValid(newRecipient)
            ? newRecipient
            : null;
    }
}

public enum AddressType
{
    Email,
    Telegram
}

public enum SendingStatus
{
    None,
    SuccessCompleted,
    CompletedWithError,
}