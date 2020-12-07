using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.Services;
using Store.Service.Common.Services;
using Store.WebAPI.Infrastructure.Models;

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
