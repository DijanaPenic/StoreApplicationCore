using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.DAL.Context;
using Store.Repositories;
using Store.Repository.Core;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Core;
using Store.Repository.Common.Core.Dapper;
using Store.Repository.Common.Repositories;
using Store.Repository.Common.Repositories.Identity.Stores;
using Store.Repositories.Identity.Stores;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.DependencyInjection
{
    public static class RepositoryExtensions
    {
        public static void AddRepositoryComponents(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Database");

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IDapperUnitOfWork, DapperUnitOfWork>(provider => new DapperUnitOfWork(connectionString));
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

            services.AddTransient<IUserStore<IUser>, ApplicationUserStore>();
            services.AddTransient<IRoleStore<IRole>, ApplicationRoleStore>();
            services.AddTransient<IApplicationAuthStore, ApplicationAuthStore>();
        }
    }
}