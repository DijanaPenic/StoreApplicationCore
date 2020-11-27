using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.WebAPI.Infrastructure;
using Store.WebAPI.Infrastructure.Models;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class EmailExtensions
    {
        public static void AddEmail(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailSender, EmailSender>();

            AuthMessageSenderOptions mailConfig = configuration.GetSection("EmailServer").Get<AuthMessageSenderOptions>();
            services.AddSingleton(mailConfig);
        }
    }
}
