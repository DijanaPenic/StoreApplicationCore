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
    public interface IBookRepository
    {
        Task<IPagedList<IBook>> FindBooksAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);

        Task<IPagedList<IBook>> FindBooksByBookstoreIdAsync(Guid bookstoreId, IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);

        Task<IEnumerable<IBook>> GetBooksAsync(IOptionsParameters options);

        public Task<IBook> FindBookByKeyAsync(Guid key, IOptionsParameters options);

        Task<ResponseStatus> UpdateBookAsync(Guid key, IBook model);

        Task<ResponseStatus> AddBookAsync(IBook model);

        Task<ResponseStatus> DeleteBookByKeyAsync(Guid key);
    }
}