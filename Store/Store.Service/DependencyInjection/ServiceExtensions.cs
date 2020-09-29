using Microsoft.Extensions.DependencyInjection;

using Store.Services;
using Store.Service.Common.Services;

namespace Store.Service.DependencyInjection
{
    // TODO - resolve other dependencies
    public static class ServiceExtensions
    {
        public static void ConfigureServiceComponents(this IServiceCollection services)
        {
            services.AddTransient<IBookstoreService, BookstoreService>();
            services.AddTransient<IBookService, BookService>();
            services.AddTransient<IGlobalSearchService, GlobalSearchService>();

            //services.AddTransient<IUserStore <IIdentityUser, Guid>, UserStore>();
            //services.AddTransient<IRoleStore <IIdentityRole, Guid>, RoleStore>();
            //services.AddTransient<IAuthStore, AuthStore>();
        }
    }
}