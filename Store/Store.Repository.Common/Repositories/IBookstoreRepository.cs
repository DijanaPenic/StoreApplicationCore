using X.PagedList;
using System.Threading.Tasks;

using Store.Model.Common.Models;
using Store.Repository.Common.Core;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Repository.Common.Repositories
{
    public interface IBookstoreRepository : IGenericRepository<IBookstore>
    {
        Task<IPagedList<IBookstore>> FindBookstoresAsync<TDestination>(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);
    }
}