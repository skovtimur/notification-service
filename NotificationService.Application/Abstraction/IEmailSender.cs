using NotificationService.Domain.Entities;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Abstraction;

public interface IEmailSender
{
    public Task SendMessage(
        string toAddress,
        string text,
        FileDto file,
        string? fromAddress,
        string? fromAddressPassword);

    public Task<bool> Exists(string email, string password);
}