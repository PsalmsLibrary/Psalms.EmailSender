using MediatR;

namespace Psalms.EmailSender.Models.Interfaces;

public interface IEmailTemplateResolver
{
    Task<object> ResolveAsync(IRequest request);
}