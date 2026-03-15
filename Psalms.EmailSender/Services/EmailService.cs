using MailKit.Net.Smtp;
using MailKit.Security;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Psalms.EmailSender.Models;
using Psalms.EmailSender.Services.Interfaces;
using System.Text.Json;

namespace Psalms.EmailSender.Services;

public partial class EmailService(
    IConfiguration config,
    IDistributedCache cache,
    IEmailHtmlGenerator generator,
    IMediator mediator) : IEmailService
{
    #region Attributes
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    #endregion

    #region IEmailService Implementation
    public async Task SendEmailConfirmationAsync(EmailConfirmationValues values, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(values.Command);

        var token = GenerateToken();

        var commandJson = SerializeCommand(values.Command);

        var tokenData = new EmailTokenData(
            values.Command.GetType().AssemblyQualifiedName!,
            commandJson
        );

        await cache.SetStringAsync(
            token,
            JsonSerializer.Serialize(tokenData, JsonOptions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = values.Expires
            }, ct);

        var confirmationLink =
            $"{config["FRONTEND_URL"]}/{values.EmailConfiguration.FrontEndPage}?token={token}";

        var html = (await generator.GenerateHtmlAsync(values.EmailConfiguration.TemplatePath, values.EmailConfiguration.EmailType, ct))
            .Replace("{confirmationLink}", confirmationLink);

        await SendAsync(values.Email, values.EmailConfiguration.Subject, html, ct);
    }

    public async Task<CommandModel> ConfirmEmailAsync(string token, CancellationToken ct)
    {
        var json = await cache.GetStringAsync(token, ct);

        if (string.IsNullOrWhiteSpace(json))
            throw new KeyNotFoundException("Token inválido ou expirado.");

        await cache.RemoveAsync(token, ct);

        var data = JsonSerializer.Deserialize<EmailTokenData>(json, JsonOptions)
            ?? throw new InvalidOperationException("Dados do token inválidos.");

        var type = Type.GetType(data.Type)
            ?? throw new InvalidOperationException("Tipo do comando não encontrado.");

        return new CommandModel(type, data.CommandJson);
    }

    public async Task ConfirmAndExecuteAsync(string token, CancellationToken ct)
    {
        var commandModel = await ConfirmEmailAsync(token, ct);

        var command = JsonSerializer.Deserialize(
            commandModel.JsonCommand,
            commandModel.Type,
            JsonOptions
        ) as IRequest
            ?? throw new InvalidOperationException("Comando inválido.");

        await mediator.Send(command, ct);
    }

    public async Task SendAsync(string to, string subject, string html, CancellationToken ct)
    {
        var message = new MimeMessage();

        message.From.Add(MailboxAddress.Parse(config["Admin:Email"]!));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = html
        }.ToMessageBody();

        using var smtp = new SmtpClient();

        try
        {
            await smtp.ConnectAsync(
                config["Email:Host"],
                int.Parse(config["Email:Port"]!),
                SecureSocketOptions.StartTls,
                ct
            );

            await smtp.AuthenticateAsync(
                config["Admin:Email"],
                config["Email:Password"],
                ct
            );

            await smtp.SendAsync(message, ct);
        }
        finally
        {
            await smtp.DisconnectAsync(true, ct);
        }
    }
    #endregion

    #region Private Methods
    private static string SerializeCommand(IRequest command) 
        => JsonSerializer.Serialize(
            command,
            command.GetType(),
            JsonOptions
        );

    private static string GenerateToken() => $"email-confirmation:{Guid.NewGuid():N}";
    #endregion
}