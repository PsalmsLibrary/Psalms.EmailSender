using MediatR;

namespace Psalms.EmailSender.Models.Commands;

public record SendConfirmationEmailCommand(IRequest Command, string Email) : IRequest<Unit>;