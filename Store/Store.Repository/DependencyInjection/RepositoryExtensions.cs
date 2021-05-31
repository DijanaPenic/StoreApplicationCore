using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.DAL.Context;
using Store.Repository.Core;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Core;
using Store.Repository.Common.Core.Dapper;
using Store.Repository.Common.Repositories.Identity.Stores;
using Store.Repositories.Identity.Stores;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.DependencyInjection
{
    public static class RepositoryExtensions
    {
        // Implement the infrastructure persistence layer with Entity Framework Core: 
        // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core
        public static void AddRepositoryComponents(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Database");

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDapperUnitOfWork, DapperUnitOfWork>();

            // DbContext scope: https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

            services.AddScoped<IUserStore<IUser>, ApplicationUserStore>();
            services.AddScoped<IRoleStore<IRole>, ApplicationRoleStore>();
            services.AddScoped<IApplicationAuthStore, ApplicationAuthStore>();
        }
    }
}