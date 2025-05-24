using System.Text.RegularExpressions;

namespace NotificationService.Domain.Validators;

public class EmailValidator : IValidator<string>
{
    public const string EmailPattern =
        "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$";

    public bool Validate(string telegramIdString) => Regex.IsMatch(telegramIdString, EmailPattern);
}