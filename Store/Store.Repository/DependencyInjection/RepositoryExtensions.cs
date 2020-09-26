using Microsoft.Extensions.DependencyInjection;

using Store.Repositories;
using Store.Repository.Core;
using Store.Repository.Common.Core;
using Store.Repository.Common.Repositories;

namespace Store.Repository.DependencyInjection
{
    // TODO - resolve other dependencies
    public static class RepositoryExtensions
    {
        public static void ConfigureRepositoryComponents(this IServiceCollection services)
        {
            services.AddTransient<IBookstoreRepository, BookstoreRepository>();
            services.AddTransient<IBookRepository, BookRepository>();
            //services.AddTransient<IGlobalSearchRepository, GlobalSearchRepository>();

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<,>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            //services.AddTransient(typeof(IdentityGenericRepository<,>)).As(typeof(IIdentityGenericRepository<>));
            //services.AddTransient<IdentityUnitOfWork>().As<IIdentityUnitOfWork>();
        }
    }
}