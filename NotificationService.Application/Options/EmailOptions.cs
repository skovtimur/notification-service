namespace NotificationService.Application.Options;

public class EmailOptions
{
    public required string From { get; set; }
    public required string Password { get; set; }
    public required string Host { get; set; }
    public required int Port { get; set; }

    public const string PathToOption = "UserSecrets:Email";
}