using MediatR;

namespace Psalms.EmailSender.Models.Interfaces;

/// <summary>
/// Defines a contract for email templates that generate email content based on a command of type TCommand.
/// </summary>
/// <typeparam name="TCommand">The type of command used to populate the template. Must implement <see cref="IRequest"/>.</typeparam>
public interface IEmailTemplate<TCommand>
    where TCommand : IRequest
{
    /// <summary>
    /// Gets the file system path to the template used by the component or service.
    /// </summary>
    string TemplatePath { get; }
    /// <summary>
    /// Gets the type of the email represented by this instance.
    /// </summary>
    string EmailType { get; }
    /// <summary>
    /// Gets the subject associated with the current instance.
    /// </summary>
    string Subject { get; }
    /// <summary>
    /// Gets the relative URL path of the associated front-end page.
    /// </summary>
    string FrontEndPage { get; }
}