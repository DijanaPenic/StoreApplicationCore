using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.Services;
using Store.Service.Common.Services;
using Store.WebAPI.Infrastructure.Models;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class EmailExtensions
    {
        public static void AddEmail(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailSenderService, EmailSenderService>();
            services.Configure<EmailSenderAuthOptions>(configuration.GetSection(EmailSenderAuthOptions.Position));
        }
    }
}
