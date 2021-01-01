using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Store.Services.Identity;
using Store.WebAPI.Infrastructure.Attributes;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class HangfireExtensions
    {
        public static void AddHangfire(this IApplicationBuilder app)
        {
            // Configure hangfire dashboard and server
            var tokenValidationParameters = (TokenValidationParameters)app.ApplicationServices.GetService(typeof(TokenValidationParameters));
            var logger = (ILogger<HangfireDashboardAuthorizationAttribute>)app.ApplicationServices.GetService(typeof(ILogger<HangfireDashboardAuthorizationAttribute>));

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationAttribute(tokenValidationParameters, logger) }    // allow only admins to access the hangfire content
            });
            app.UseHangfireServer();

            // Configure hangfire daily job - recurring job at 9.00 AM Daily       
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                var authManager = (ApplicationAuthManager)scope.ServiceProvider.GetService(typeof(ApplicationAuthManager)); // ApplicationUserManager input parameter is a scoped service
                RecurringJob.AddOrUpdate(() => authManager.RemoveExpiredRefreshTokensAsync(), "0 9 * * *");
            }
        }
    }
}