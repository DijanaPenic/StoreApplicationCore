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
    public interface IBookService
    {
        Task<IEnumerable<IBook>> GetBooksAsync(IOptionsParameters options);

        Task<IBook> FindBookByIdAsync(Guid bookId, IOptionsParameters options);

        Task<IPagedList<IBook>> FindBooksAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);

        Task<ResponseStatus> UpdateBookAsync(IBook book);

        Task<ResponseStatus> UpdateBookAsync(Guid bookId, IBook book);

        Task<ResponseStatus> AddBookAsync(IBook book);

        Task<ResponseStatus> DeleteBookAsync(Guid bookId);
    }
}