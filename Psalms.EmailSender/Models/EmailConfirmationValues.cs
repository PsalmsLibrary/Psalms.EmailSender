using MediatR;

namespace Psalms.EmailSender.Models;

public record EmailConfirmationValues
(
    IRequest Command,
    string Email,
    dynamic EmailConfiguration,
    TimeSpan Expires = default
)
{
    public TimeSpan Expires { get; init; } =
        Expires == default ? TimeSpan.FromMinutes(5) : Expires;
}