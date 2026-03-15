using MediatR;
using Psalms.EmailSender.Models.Commands;
using Psalms.EmailSender.Models.Interfaces;
using Psalms.EmailSender.Services.Interfaces;

namespace Psalms.EmailSender.Models.Handlers;

public class SendConfirmationEmailHandler(IEmailService service, IEmailTemplateResolver resolver) : IRequestHandler<SendConfirmationEmailCommand, Unit>
{
    public async Task<Unit> Handle(SendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        dynamic template = await resolver.ResolveAsync(request.Command);

        var values = new EmailConfirmationValues
            (
                request.Command,
                request.Email,
                template
            );

        await service.SendEmailConfirmationAsync(values);

        return Unit.Value;
    }
}