using Hangfire;
using Hangfire.PostgreSql;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

using Store.Cache.DependencyInjection;
using Store.WebAPI.Application.Startup;
using Store.WebAPI.Application.Startup.Extensions;
using Store.Service.DependencyInjection;
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

            // Authorization configuration
            services.AddAuthorization(Configuration);

            // Swagger configuration
            services.AddSwagger();

            // Hangfire configuration
            services.AddHangfire(config => config.UsePostgreSqlStorage(Configuration.GetConnectionString("Database")));

            // Messaging configuration (SMS, voice, email)
            services.AddMessaging(Configuration);

            // File provider configuration
            services.AddFileProvider(Configuration);

            // Controller configuration
            services.AddControllers(options => 
            {
                options.ValueProviderFactories.Add(new SnakeCaseQueryValueProviderFactory());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.AddHangfire();

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