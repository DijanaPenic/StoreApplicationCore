using System;
using System.Threading.Tasks;
using X.PagedList;

using Store.Model.Common.Models;
using Store.Repository.Common.Core;

namespace Store.Repository.Common.Repositories
{
    public interface IBookRepository : IGenericRepository<IBook>
    {
        Task<IPagedList<IBook>> FindByBookstoreIdAsync(Guid bookstoreId, string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize);
    }
}