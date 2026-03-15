using MediatR;

namespace Psalms.EmailSender.Models.Interfaces;

public interface IEmailTemplate<TCommand>
    where TCommand : IRequest
{
    string TemplatePath { get; }
    string EmailType { get; }
    string Subject { get; }
    string FrontEndPage { get; }
}