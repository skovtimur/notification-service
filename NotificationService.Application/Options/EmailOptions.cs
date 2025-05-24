namespace NotificationService.Application.Options;

public class EmailOptions
{
    public string From { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }

    public const string PathToOption = "UserSecrets:Email";
}