using MediatR;
using Psalms.EmailSender.Models.Interfaces;

namespace Psalms.EmailSender.Models;

internal class EmailTemplateResolver(IServiceProvider serviceProvider) : IEmailTemplateResolver
{
    public Task<object> ResolveAsync(IRequest command)
    {
        var commandType = command.GetType();

        var templateType = typeof(IEmailTemplate<>).MakeGenericType(commandType);

        var service = serviceProvider.GetService(templateType) 
            ?? throw new ArgumentNullException("Template não registrado.");

        return Task.FromResult(service);
    }
}