using NotificationService.Domain.Exceptions;
using Quartz;

namespace NotificationService.Application.Extensions;

public static class JobDataExtensions
{
    public static T GetValueOrThrowException<T>(this JobDataMap jobData, string key)
    {
        if (jobData.TryGetValue(key, out var obj)
            && obj is T value)
            return value;

        throw new NoImportantDataException($"The Value must be of type {typeof(T)}");
    }
}