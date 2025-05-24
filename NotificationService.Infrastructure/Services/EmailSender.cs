using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationService.Application.Abstraction;
using NotificationService.Application.Options;
using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly MailKit.Net.Smtp.SmtpClient _defaultClient;
    private readonly EmailOptions _config;

    private readonly string _titlePattern;
    private readonly string _emailMessagePattern;

    public EmailSender(IOptions<EmailOptions> options, IConfiguration configuration, ILogger<EmailSender> logger)
    {
        _logger = logger;
        _config = options.Value;

        _titlePattern = configuration.GetRequiredSection("EmailTitlePattern").Value;
        _emailMessagePattern = configuration.GetRequiredSection("EmailMessagePattern").Value;

        _defaultClient = new MailKit.Net.Smtp.SmtpClient();
        _defaultClient.Connect(_config.Host, _config.Port, MailKit.Security.SecureSocketOptions.StartTls);
        _defaultClient.Authenticate(_config.From, _config.Password);
    }

    public async Task SendMessage(
        string toAddress,
        string text,
        FileDto file,
        string? fromAddress = null,
        string? fromAddressPassword = null)
    {
        var client = _defaultClient;
        var from = _config.From;

        if (string.IsNullOrEmpty(fromAddress) == false
            && string.IsNullOrEmpty(fromAddressPassword) == false)
        {
            client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(_config.Host, _config.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(fromAddress, fromAddressPassword);

            from = fromAddress;
        }

        var body = new BodyBuilder
        {
            HtmlBody = _emailMessagePattern.Replace("{TEXT}", text)
        };
        if (file != null && file.Weigh > 0)
            body.Attachments.Add(file.FileName, file.FileContent);

        using var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Admin", from));
        message.To.Add(new MailboxAddress("", toAddress));

        message.Subject = _titlePattern;
        message.Body = body.ToMessageBody();

        await client.SendAsync(message);
    }

    public async Task<bool> Exists(string email, string password)
    {
        try
        {
            var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(_config.Host, _config.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(email, password);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.Message);
            return false;
        }
    }
}