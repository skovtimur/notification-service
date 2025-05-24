using Microsoft.Extensions.Configuration;
using NotificationService.Domain.Exceptions;

namespace NotificationService.Application.Extensions;

public static class ConfigurationExtensions
{
    public static string GetRequredValue(this IConfigurationManager section, string key)
    {
        var value = section.GetRequiredSection(key).Value;

        if (string.IsNullOrEmpty(value))
            throw new IsNullOrEmptyException($"Configuration of the '{key}' wasn't found");

        return value;
    }
}