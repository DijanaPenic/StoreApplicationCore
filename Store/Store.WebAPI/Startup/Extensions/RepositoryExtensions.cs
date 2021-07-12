using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.DAL.Context;
using Store.Repository.Core;
using Store.Repository.Common.Core;
using Store.Repository.Common.Repositories.Identity.Stores;
using Store.Repositories.Identity.Stores;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class RepositoryExtensions
    {
        // Implement the infrastructure persistence layer with Entity Framework Core: 
        // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core
        public static void AddRepositoryComponents(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // DbContext scope: https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/
            string dbConnectionString = configuration.GetConnectionString("LocalDatabase");
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(dbConnectionString).UseSnakeCaseNamingConvention());

            services.AddScoped<IUserStore<IUser>, ApplicationUserStore>();
            services.AddScoped<IRoleStore<IRole>, ApplicationRoleStore>();
            services.AddScoped<IApplicationAuthStore, ApplicationAuthStore>();
        }
    }
}