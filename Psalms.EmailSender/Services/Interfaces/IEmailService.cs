using Psalms.EmailSender.Models;

namespace Psalms.EmailSender.Services.Interfaces;

public interface IEmailService
{
    Task ConfirmAndExecuteAsync(string token);
    Task<CommandModel> ConfirmEmailAsync(string token);
    Task SendAsync(string to, string subject, string html);
    Task SendEmailConfirmationAsync(EmailConfirmationValues values);
}
