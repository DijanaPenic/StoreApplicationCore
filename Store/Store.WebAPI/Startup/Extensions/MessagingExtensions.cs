using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.Messaging.Options;
using Store.Messaging.Services;
using Store.Messaging.Services.Common;
using Store.Messaging.Templates.Services.Email;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class MessagingExtensions
    {
        public static void AddMessagingServices(this IServiceCollection services, IConfiguration configuration)
        {
            // SMS & Voice
            services.AddTransient<ISmsService, SmsService>();
            services.AddTransient<IVoiceService, VoiceService>();
            services.Configure<TwilioAuthOptions>(configuration.GetSection(TwilioAuthOptions.Position));

            // Email
            services.AddTransient<IEmailService, EmailService>();
            services.Configure<SendGridAuthOptions>(configuration.GetSection(SendGridAuthOptions.Position));

            services.AddTransient<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddRazorPages();
        }
    }
}
