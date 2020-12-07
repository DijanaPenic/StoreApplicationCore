using Microsoft.Extensions.DependencyInjection;

using Store.Services;
using Store.Service.Common.Services;

namespace Store.Service.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static void AddServiceComponents(this IServiceCollection services)
        {
            services.AddTransient<IBookstoreService, BookstoreService>();
            services.AddTransient<IBookService, BookService>();
            services.AddTransient<IGlobalSearchService, GlobalSearchService>();
            services.AddTransient<ICountriesService, CountriesService>();
        }
    }
}