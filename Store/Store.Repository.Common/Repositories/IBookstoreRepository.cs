using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models;

namespace Store.Repository.Common.Repositories
{
    public interface IBookstoreRepository
    {
        Task<IPagedList<IBookstore>> FindBookstoresAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);

        Task<IEnumerable<IBookstore>> FindBookstoresAsync(IFilteringParameters filter, ISortingParameters sorting, IOptionsParameters options);

        Task<IEnumerable<IBookstore>> GetBookstoresAsync(IOptionsParameters options);

        Task<IBookstore> FindBookstoreByKeyAsync(Guid key, IOptionsParameters options);

        Task<ResponseStatus> UpdateBookstoreAsync(Guid key, IBookstore model);

        Task<ResponseStatus> AddBookstoreAsync(IBookstore model);

        Task<ResponseStatus> DeleteBookstoreByKeyAsync(Guid key);
    }
}