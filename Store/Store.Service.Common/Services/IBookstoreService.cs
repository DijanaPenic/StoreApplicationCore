using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Common.Enums;
using Store.Model.Common.Models;

namespace Store.Service.Common.Services
{
    public interface IBookstoreService
    {
        Task<IBookstore> FindBookstoreByIdAsync(Guid bookstoreId, params string[] includeProperties);

        Task<IPagedList<IBook>> FindBooksByBookstoreIdAsync(Guid bookstoreId, string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize);

        Task<IEnumerable<IBookstore>> GetBookstoresAsync(params string[] includeProperties);

        Task<IPagedList<IBookstore>> FindBookstoresAsync(string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize, params string[] includeProperties);

        Task<ResponseStatus> UpdateBookstoreAsync(IBookstore bookstore);

        Task<ResponseStatus> UpdateBookstoreAsync(Guid bookstoreId, IBookstore bookstore);

        Task<ResponseStatus> InsertBookstoreAsync(IBookstore bookstore);

        Task<ResponseStatus> DeleteBookstoreAsync(Guid bookstoreId);
    }
}