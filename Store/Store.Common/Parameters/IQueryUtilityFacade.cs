using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Filtering;

namespace Store.Common.Parameters
{
    public interface IQueryUtilityFacade
    {
        ISortingFactory CreateSortingFactory();

        IFilteringFactory CreateFilteringFactory();

        IPagingFactory CreatePagingFactory();

        IOptionsFactory CreateOptionsFactory();
    }
}