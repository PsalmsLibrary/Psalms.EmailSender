using MediatR;

namespace Psalms.EmailSender.Models;

/// <summary>
/// Represents the values required to generate and send an email confirmation, including the command to execute,
/// recipient email address, email configuration, and expiration period.
/// </summary>
/// <remarks>The default expiration period is five minutes if no value is provided. The actual behavior of the
/// email confirmation depends on the implementation of the associated command and email configuration.</remarks>
/// <param name="Command">The command associated with the email confirmation request. This is typically used to trigger the confirmation
/// logic.</param>
/// <param name="Email">The email address to which the confirmation will be sent. Cannot be null or empty.</param>
/// <param name="EmailConfiguration">The configuration settings used to compose or send the confirmation email. The structure and required properties
/// depend on the email provider or system in use.</param>
/// <param name="Expires">The duration after which the confirmation expires. If not specified, defaults to five minutes.</param>
public record EmailConfirmationValues
(
    IRequest Command,
    string Email,
    dynamic EmailConfiguration,
    TimeSpan Expires = default
)
{
    public TimeSpan Expires { get; init; } =
        Expires == default ? TimeSpan.FromMinutes(5) : Expires;
}