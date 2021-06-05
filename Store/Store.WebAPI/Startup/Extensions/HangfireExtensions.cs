using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Store.Services.Identity;
using Store.WebAPI.Infrastructure.Authorization.Attributes;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class HangfireExtensions
    {
        public static void AddHangfire(this IApplicationBuilder app)
        {
            // Configure hangfire dashboard and server
            TokenValidationParameters tokenValidationParameters = (TokenValidationParameters)app.ApplicationServices.GetService(typeof(TokenValidationParameters));
            ILogger<HangfireDashboardAuthorizationAttribute> logger = (ILogger<HangfireDashboardAuthorizationAttribute>)app.ApplicationServices.GetService(typeof(ILogger<HangfireDashboardAuthorizationAttribute>));

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationAttribute(tokenValidationParameters, logger) }    // allow only admins to access the hangfire content
            });

            BackgroundJobServerOptions serverOptions = new BackgroundJobServerOptions
            {
                WorkerCount = 1    //Hangfire's default worker count is 20, which opens 20 connections simultaneously. WorkerCount = 1 will open 6 connections.            
            };
            app.UseHangfireServer(serverOptions);

            // Configure hangfire daily job - recurring job at 9.00 AM Daily       
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            ApplicationAuthManager authManager = (ApplicationAuthManager)scope.ServiceProvider.GetService(typeof(ApplicationAuthManager)); // ApplicationUserManager input parameter is a scoped service
            RecurringJob.AddOrUpdate(() => authManager.RemoveExpiredRefreshTokensAsync(), "0 9 * * *");
        }
    }
}