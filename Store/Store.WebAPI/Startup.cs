using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

using Store.WebAPI.Application.Startup.Policies;
using Store.WebAPI.Application.Startup.Providers;
using Store.WebAPI.Application.Startup.Extensions;

namespace Store.WebAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Auto Mapper configuration
            services.AddAutoMapperServices();

            // Common configuration
            services.AddCommonComponents();

            // Repository configuration
            services.AddRepositoryComponents(_configuration);

            // Service configuration
            services.AddServiceComponents();
            
            // Cache configuration
            services.AddCacheComponents(_configuration);

            // Authentication configuration
            services.AddAuthenticationServices(_configuration);

            // Authorization configuration
            services.AddAuthorizationServices(_configuration);

            // Swagger configuration
            services.AddSwaggerServices();

            // Hangfire configuration
            //services.AddHangfireServices(config => config.UsePostgreSqlStorage(Configuration.GetConnectionString("Database")));

            // Messaging configuration (SMS, voice, email)
            services.AddMessagingServices(_configuration);

            // File provider configuration
            services.AddFileProviderServices(_configuration);

            // Controller configuration
            services.AddControllers(options =>
            {
                options.ValueProviderFactories.Add(new SnakeCaseQueryValueProviderFactory());
                options.ModelBinderProviders.Insert(0, new GuidEntityBinderProvider());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // FluentValidation configuration
            services.AddFluentValidationServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Disabled Hangfire for easier sql log troubleshooting.
            //app.AddHangfire();

            app.AddSwagger();

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