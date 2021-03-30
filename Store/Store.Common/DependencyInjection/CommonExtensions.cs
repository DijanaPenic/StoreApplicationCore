using Microsoft.Extensions.DependencyInjection;

using Store.Common.Parameters;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Common.DependencyInjection
{
    public static class CommonExtensions
    {
        public static void AddCommonComponents(this IServiceCollection services)
        {
            services.AddScoped<IQueryUtilityFacade, QueryUtilityFacade>();

            services.AddScoped<IFilteringFactory, FilteringFactory>();
            services.AddScoped<ISortingFactory, SortingFactory>();
            services.AddScoped<IPagingFactory, PagingFactory>();
            services.AddScoped<IOptionsFactory, OptionsFactory>();

            services.AddScoped<IFilteringParameters, FilteringParameters>();
            services.AddScoped<IGlobalFilteringParameters, GlobalFilteringParameters>();
            services.AddScoped<IUserFilteringParameters, UserFilteringParameters>();
        }
    }
}