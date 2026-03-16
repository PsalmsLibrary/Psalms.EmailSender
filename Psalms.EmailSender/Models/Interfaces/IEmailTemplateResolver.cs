using MediatR;

namespace Psalms.EmailSender.Models.Interfaces;

/// <summary>
/// Defines a contract for resolving email templates based on a specified request.
/// </summary>
/// <remarks>Implementations of this interface are responsible for retrieving or generating email templates
/// according to the details provided in the request. This interface is typically used in scenarios where email content
/// must be dynamically selected or constructed at runtime, such as in notification or messaging systems.</remarks>
public interface IEmailTemplateResolver
{
    /// <summary>
    /// Asynchronously resolves the specified request and returns the result.
    /// </summary>
    /// <param name="request">The request to be resolved. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the resolved object for the request.</returns>
    Task<object> ResolveAsync(IRequest request);
}