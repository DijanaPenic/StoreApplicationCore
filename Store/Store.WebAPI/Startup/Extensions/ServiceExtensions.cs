using Microsoft.Extensions.DependencyInjection;

using Store.Services;
using Store.Service.Common.Services;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServiceComponents(this IServiceCollection services)
        {
            services.AddScoped<IBookstoreService, BookstoreService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IGlobalSearchService, GlobalSearchService>();
            services.AddScoped<ICountriesService, CountriesService>();
        }
    }
}