using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Store.Services;
using Store.Services.Identity;
using Store.Service.Common.Services;
using Store.Model.Common.Models.Identity;

namespace Store.Service.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static void AddServiceComponents(this IServiceCollection services)
        {
            services.AddTransient<IBookstoreService, BookstoreService>();
            services.AddTransient<IBookService, BookService>();
            services.AddTransient<IGlobalSearchService, GlobalSearchService>();

            services.AddTransient<IUserStore<IUser>, ApplicationUserStore>();
            services.AddTransient<IRoleStore<IRole>, ApplicationRoleStore>();
        }
    }
}