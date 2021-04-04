using System;
using Microsoft.Extensions.DependencyInjection;

using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Filtering;

namespace Store.Common.Parameters
{
    public class QueryUtilityFacade : IQueryUtilityFacade
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryUtilityFacade(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFilteringFactory CreateFilteringFactory()
        {
            return _serviceProvider.GetService<IFilteringFactory>();
        }

        public IPagingFactory CreatePagingFactory()
        {
            return _serviceProvider.GetService<IPagingFactory>();
        }

        public ISortingFactory CreateSortingFactory()
        {
            return _serviceProvider.GetService<ISortingFactory>();
        }

        public IOptionsFactory CreateOptionsFactory()
        {
            return _serviceProvider.GetService<IOptionsFactory>();
        }
    }
}