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
    public interface IBookRepository : IRepository<IBook, Guid>
    {
        Task<IPagedList<IBook>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null);

        Task<IPagedList<IBook>> FindByBookstoreIdAsync(Guid bookstoreId, IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);
    }
}