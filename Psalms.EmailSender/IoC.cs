using Microsoft.Extensions.DependencyInjection;
using Psalms.EmailSender.Models;
using Psalms.EmailSender.Models.Interfaces;
using Psalms.EmailSender.Services;
using Psalms.EmailSender.Services.Interfaces;

namespace Psalms.EmailSender;

public static class IoC
{
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