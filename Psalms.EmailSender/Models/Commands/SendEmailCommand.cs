using MediatR;

namespace Psalms.EmailSender.Models.Commands;

public record SendEmailCommand(string To, string Subject, string HtmlPath, string EmailType) : IRequest;