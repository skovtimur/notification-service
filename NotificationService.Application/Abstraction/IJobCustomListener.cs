using Quartz;

namespace NotificationService.Application.Abstraction;

public interface IJobCustomListener : IJobListener
{
    public void SetName(string initName);
}