using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Models;
using Store.Model.Common.Models;
using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Service.Common.Services
{
    public interface IBookstoreService
    {
        Task<BookstoreExtendedDTO> FindBookstoreByKeyAsync(Guid bookstoreId, IOptionsParameters options);

        Task<IEnumerable<BookstoreExtendedDTO>> GetBookstoresAsync(ISortingParameters sorting, IOptionsParameters options);

        Task<IPagedList<BookstoreExtendedDTO>> FindBookstoresAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);

        Task<ResponseStatus> UpdateBookstoreAsync(IBookstore bookstore);

        Task<ResponseStatus> InsertBookstoreAsync(IBookstore bookstore);

        Task<ResponseStatus> DeleteBookstoreAsync(Guid bookstoreId);
    }
}