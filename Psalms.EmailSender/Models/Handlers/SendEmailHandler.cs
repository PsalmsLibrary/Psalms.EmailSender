using MediatR;
using Psalms.EmailSender.Models.Commands;
using Psalms.EmailSender.Services;
using Psalms.EmailSender.Services.Interfaces;

namespace Psalms.EmailSender.Models.Handlers;

public class SendEmailHandler(IEmailService service, IEmailHtmlGenerator htmlGenerator) : IRequestHandler<SendEmailCommand>
{
    public async Task Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        var html = await htmlGenerator.GenerateHtmlAsync(request.HtmlPath, request.EmailType, cancellationToken);

        await service.SendAsync(request.To, request.Subject, html, cancellationToken);
    }
}