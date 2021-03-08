using Autofac;

using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Common.Parameters
{
    public class QueryUtilityFacade : IQueryUtilityFacade
    {
        private readonly ILifetimeScope _lifeTimeScope;

        public QueryUtilityFacade(ILifetimeScope lifeTimeScope)
        {
            _lifeTimeScope = lifeTimeScope;
        }

        public IFilteringFactory CreateFilteringFactory()
        {
            return _lifeTimeScope.Resolve<IFilteringFactory>();
        }

        public IPagingFactory CreatePagingFactory()
        {
            return _lifeTimeScope.Resolve<IPagingFactory>();
        }

        public ISortingFactory CreateSortingFactory()
        {
            return _lifeTimeScope.Resolve<ISortingFactory>();
        }

        public IOptionsFactory CreateOptionsFactory()
        {
            return _lifeTimeScope.Resolve<IOptionsFactory>();
        }
    }
}