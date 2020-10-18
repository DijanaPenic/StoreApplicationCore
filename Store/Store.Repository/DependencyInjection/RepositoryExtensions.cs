using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Store.DAL.Context;
using Store.Repositories;
using Store.Repository.Core;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Core;
using Store.Repository.Common.Core.Dapper;
using Store.Repository.Common.Repositories;

namespace Store.Repository.DependencyInjection
{
    public static class RepositoryExtensions
    {
        public static void AddRepositoryComponents(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<IBookstoreRepository, BookstoreRepository>();
            services.AddTransient<IBookRepository, BookRepository>();

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<,>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDapperUnitOfWork, DapperUnitOfWork>(provider => new DapperUnitOfWork(connectionString));
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
        }
    }
}