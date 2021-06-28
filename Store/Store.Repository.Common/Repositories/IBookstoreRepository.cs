using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Models;
using Store.Model.Common.Models;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Repository.Common.Repositories
{
    public interface IBookstoreRepository : IRepository<IBookstore, Guid>
    {
        Task<IPagedList<BookstoreExtendedDto>> FindExtendedAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null);

        Task<BookstoreExtendedDto> FindExtendedByKeyAsync(Guid key, IOptionsParameters options = null);

        Task<IEnumerable<BookstoreExtendedDto>> GetExtendedAsync(ISortingParameters sorting, IOptionsParameters options = null);
    }
}