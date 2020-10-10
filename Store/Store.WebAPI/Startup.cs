using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.WebAPI.Mapper;
using Store.Repository.Mapper;
using Store.Repository.DependencyInjection;
using Store.Cache.DependencyInjection;
using Store.Service.DependencyInjection;
using Store.Model.Common.Models.Identity;

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
            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperWebApiProfile>();
                cfg.AddProfile<AutoMapperRepositoryProfile>();

                cfg.AddExpressionMapping();
            });

            IMapper mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);

            // Repository configuration
            services.ConfigureRepositoryComponents(Configuration.GetConnectionString("DatabaseConnection"));

            // Service configuration
            services.ConfigureServiceComponents();
            
            // Cache configuration
            services.ConfigureCacheComponents(Configuration.GetConnectionString("RedisConnection"));

            // Identity configuration
            services.AddIdentity<IUser, IRole>().AddDefaultTokenProviders();

            // TODO - resolve registration of other services 
            //// register access token format
            //builder.RegisterType<TicketDataFormat>().As<ISecureDataFormat<AuthenticationTicket>>();
            //builder.RegisterType<TicketSerializer>().As<IDataSerializer<AuthenticationTicket>>();
            //builder.Register(c => new DpapiDataProtectionProvider().Create("ASP.NET Identity")).As<IDataProtector>();

            //// register identity instances
            //builder.RegisterType<ApplicationUserManager>();
            //builder.RegisterType<ApplicationRoleManager>();
            //builder.RegisterType<ApplicationSignInManager>();
            //builder.Register(c => HttpContext.Current.GetOwinContext().Authentication); // IAuthenticationManager instance
            //builder.RegisterType<AuthorizationServerProvider>().SingleInstance();

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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}