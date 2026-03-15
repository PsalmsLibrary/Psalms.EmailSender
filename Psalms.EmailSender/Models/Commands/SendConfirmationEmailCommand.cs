using MediatR;

namespace Psalms.EmailSender.Models.Commands;

/// <summary>
/// Represents a command to send a confirmation email to a specified recipient.
/// </summary>
/// <param name="Command">The request data associated with the confirmation email operation. Cannot be null.</param>
/// <param name="Email">The email address of the recipient to whom the confirmation email will be sent. Cannot be null or empty.</param>
public record SendConfirmationEmailCommand(IRequest Command, string Email) : IRequest<Unit>;