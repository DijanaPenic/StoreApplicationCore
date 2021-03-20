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

namespace Store.Service.Common.Services
{
    public interface IBookstoreService
    {
        Task<IBookstore> FindBookstoreByIdAsync(Guid bookstoreId, IOptionsParameters options);

        Task<IPagedList<IBook>> FindBooksByBookstoreIdAsync(Guid bookstoreId, IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);

        Task<IEnumerable<IBookstore>> GetBookstoresAsync(IOptionsParameters options);

        Task<IPagedList<IBookstore>> FindBookstoresAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);

        Task<ResponseStatus> UpdateBookstoreAsync(IBookstore bookstore);

        Task<ResponseStatus> UpdateBookstoreAsync(Guid bookstoreId, IBookstore bookstore);

        Task<ResponseStatus> InsertBookstoreAsync(IBookstore bookstore);

        Task<ResponseStatus> DeleteBookstoreAsync(Guid bookstoreId);
    }
}