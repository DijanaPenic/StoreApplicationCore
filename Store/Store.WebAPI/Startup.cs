using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.WebAPI.Identity;
using Store.WebAPI.Application.Startup;
using Store.Cache.DependencyInjection;
using Store.Service.DependencyInjection;
using Store.Model.Common.Models.Identity;
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
            services.AddRepositoryComponents(Configuration.GetConnectionString("DatabaseConnection"));

            // Service configuration
            services.AddServiceComponents();
            
            // Cache configuration
            services.AddCacheComponents(Configuration.GetConnectionString("RedisConnection"));

            // Identity configuration
            services.AddIdentity<IUser, IRole>()
                    .AddUserManager<ApplicationUserManager>()
                    .AddRoleManager<ApplicationRoleManager>()
                    .AddDefaultTokenProviders();

            // JWT authentication configuration
            services.AddAuthentication(Configuration.GetSection("JwtTokenConfig"));

            // Controller configuration
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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