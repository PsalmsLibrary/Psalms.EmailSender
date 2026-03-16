using MediatR;
using Psalms.EmailSender.Models.Commands;
using Psalms.EmailSender.Models.Interfaces;
using Psalms.EmailSender.Services.Interfaces;

namespace Psalms.EmailSender.Models.Handlers;

/// <summary>
/// Handles the SendConfirmationEmailCommand by generating and sending a confirmation email using the provided email
/// service and template resolver.
/// </summary>
/// <param name="service">The email service used to send confirmation emails.</param>
/// <param name="resolver">The email template resolver used to obtain the appropriate email template for the confirmation email.</param>
public class SendConfirmationEmailHandler(IEmailService service, IEmailTemplateResolver resolver) : IRequestHandler<SendConfirmationEmailCommand, Unit>
{
    public async Task<Unit> Handle(SendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        dynamic template = await resolver.ResolveAsync(request.Command);

        var values = new EmailConfirmationValues
            (
                request.Command,
                request.Email,
                template,
                request.TokenExpires
            );

        await service.SendEmailConfirmationAsync(values, cancellationToken);

        return Unit.Value;
    }
}