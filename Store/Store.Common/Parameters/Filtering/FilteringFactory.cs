using Autofac;

namespace Store.Common.Parameters.Filtering
{
    public class FilteringFactory : IFilteringFactory
    {
        private readonly ILifetimeScope _lifeTimeScope;

        public FilteringFactory(ILifetimeScope lifeTimeScope)
        {
            _lifeTimeScope = lifeTimeScope;
        }

        public T Create<T>() where T : IFilteringParameters
        {
            return _lifeTimeScope.Resolve<T>();
        }
    }
}