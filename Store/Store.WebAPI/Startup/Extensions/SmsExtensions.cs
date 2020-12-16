using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.Messaging.Options;
using Store.Messaging.Services;
using Store.Messaging.Services.Common;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class SmsExtensions
    {
        public static void AddSms(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISmsSenderService, SmsSenderService>();
            services.Configure<SmsSenderAuthOptions>(configuration.GetSection(SmsSenderAuthOptions.Position));
        }
    }
}
