using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Common.Enums;
using Store.Model.Common.Models;

namespace Store.Service.Common.Services
{
    public interface IBookService
    {
        Task<IEnumerable<IBook>> GetBooksAsync(params string[] includeProperties);

        Task<IBook> FindBookByIdAsync(Guid bookId, params string[] includeProperties);

        Task<IPagedList<IBook>> FindBooksAsync(string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize, params string[] includeProperties);

        Task<ResponseStatus> UpdateBookAsync(IBook book);

        Task<ResponseStatus> UpdateBookAsync(Guid bookId, IBook book);

        Task<ResponseStatus> AddBookAsync(IBook book);

        Task<ResponseStatus> DeleteBookAsync(Guid bookId);
    }
}