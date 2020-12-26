using Hangfire;
using Hangfire.PostgreSql;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Store.Services.Identity;
using Store.Service.DependencyInjection;
using Store.WebAPI.Application.Startup;
using Store.WebAPI.Application.Startup.Extensions;
using Store.WebAPI.Infrastructure.Attributes;
using Store.Cache.DependencyInjection;
using Store.Repository.DependencyInjection;

namespace Store.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Auto Mapper configuration
            services.AddAutoMapper();

            // Repository configuration
            services.AddRepositoryComponents(Configuration);

            // Service configuration
            services.AddServiceComponents();
            
            // Cache configuration
            services.AddCacheComponents(Configuration);

            // Authentication configuration
            services.AddAuthentication(Configuration);

            // Swagger configuration
            services.AddSwagger();

            // Hangfire configuration
            services.AddHangfire(config => config.UsePostgreSqlStorage(Configuration.GetConnectionString("Database")));

            // Email configuration
            services.AddEmail(Configuration);

            // SMS configuration
            services.AddSms(Configuration);

            // File provider configuration
            services.AddFileProvider(Configuration);

            // Controller configuration
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(swaggerOptions =>
            {
                swaggerOptions.SerializeAsV2 = true;
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(swaggerOptions =>
            {
                swaggerOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Store API V1");
                swaggerOptions.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}