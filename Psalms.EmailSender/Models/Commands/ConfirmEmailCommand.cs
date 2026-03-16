using MediatR;

namespace Psalms.EmailSender.Models.Commands;

public record ConfirmEmailCommand(string Token) : IRequest<Unit>;