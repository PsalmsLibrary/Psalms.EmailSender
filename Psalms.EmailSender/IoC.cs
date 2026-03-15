using Microsoft.Extensions.DependencyInjection;
using Psalms.EmailSender.Models;
using Psalms.EmailSender.Models.Interfaces;
using Psalms.EmailSender.Services;
using Psalms.EmailSender.Services.Interfaces;

namespace Psalms.EmailSender;

public static class IoC
{
    /// <summary>
    /// Adds the Psalms email sender services and related dependencies to the specified service collection.
    /// </summary>
    /// <remarks>This method registers the email service, HTML generator, and template resolver as scoped
    /// dependencies, and configures MediatR to scan the assembly containing the email service interfaces. Call this
    /// method during application startup to enable email sending functionality.</remarks>
    /// <param name="services">The service collection to which the email sender services will be added. Cannot be null.</param>
    public static void AddPsalmsEmailSender(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailHtmlGenerator, EmailHtmlGenerator>();
        services.AddScoped<IEmailTemplateResolver, EmailTemplateResolver>();

        services.AddMediatR(
            cfg => {
                cfg.RegisterServicesFromAssembly(typeof(IEmailService).Assembly);
            });
    }
}