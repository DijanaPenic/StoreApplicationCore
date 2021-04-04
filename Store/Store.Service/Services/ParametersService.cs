using Store.Common.Parameters;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Filtering;

namespace Store.Services
{
    internal abstract class ParametersService
    {
        private IOptionsFactory _optionsFactory = null;
        private IPagingFactory _pagingFactory = null;
        private ISortingFactory _sortingFactory = null;
        private IFilteringFactory _filteringFactory = null;

        protected IQueryUtilityFacade QueryUtilityFacade { get; }

        protected IOptionsFactory OptionsFactory
        {
            get
            {
                if (_optionsFactory == null)
                {
                    _optionsFactory = QueryUtilityFacade.CreateOptionsFactory();
                }
                return _optionsFactory;
            }
        }

        protected IFilteringFactory FilteringFactory
        {
            get
            {
                if (_filteringFactory == null)
                {
                    _filteringFactory = QueryUtilityFacade.CreateFilteringFactory();
                }
                return _filteringFactory;
            }
        }

        protected IPagingFactory PagingFactory
        {
            get
            {
                if (_pagingFactory == null)
                {
                    _pagingFactory = QueryUtilityFacade.CreatePagingFactory();
                }
                return _pagingFactory;
            }
        }

        protected ISortingFactory SortingFactory
        {
            get
            {
                if (_sortingFactory == null)
                {
                    _sortingFactory = QueryUtilityFacade.CreateSortingFactory();
                }
                return _sortingFactory;
            }
        }

        public ParametersService(IQueryUtilityFacade queryUtilityFacade)
        {
            QueryUtilityFacade = queryUtilityFacade;
        }
    }
}