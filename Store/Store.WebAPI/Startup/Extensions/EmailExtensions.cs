using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.Messaging.Options;
using Store.Messaging.Services;
using Store.Messaging.Services.Common;
using Store.EmailTemplate.Services;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class EmailExtensions
    {
        public static void AddEmail(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailSenderService, EmailSenderService>();
            services.Configure<EmailSenderAuthOptions>(configuration.GetSection(EmailSenderAuthOptions.Position));

            services.AddTransient<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddRazorPages();
        }
    }
}
