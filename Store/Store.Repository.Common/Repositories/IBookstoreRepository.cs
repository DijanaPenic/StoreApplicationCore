using System;
using System.Threading.Tasks;
using X.PagedList;

using Store.Model.Common.Models;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Repository.Common.Repositories
{
    public interface IBookstoreRepository : IRepository<IBookstore, Guid>
    {
        Task<IPagedList<IBookstore>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null);
    }
}