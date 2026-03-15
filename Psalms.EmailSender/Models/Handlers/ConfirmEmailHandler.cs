using MediatR;
using Psalms.EmailSender.Models.Commands;
using Psalms.EmailSender.Services.Interfaces;

namespace Psalms.EmailSender.Models.Handlers;

public class ConfirmEmailHandler(IEmailService service) : IRequestHandler<ConfirmEmailCommand, Unit>
{
    public async Task<Unit> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        await service.ConfirmAndExecuteAsync(request.Token);

        return Unit.Value;
    }
}
