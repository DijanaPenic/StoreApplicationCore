using System;
using Microsoft.Extensions.DependencyInjection;

namespace Store.Common.Parameters.Filtering
{
    public class FilteringFactory : IFilteringFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FilteringFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Create<T>() where T : IFilteringParameters
        {
            return _serviceProvider.GetService<T>();
        }
    }
}