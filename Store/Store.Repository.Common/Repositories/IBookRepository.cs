using System;
using System.Threading.Tasks;
using X.PagedList;

using Store.Model.Common.Models;
using Store.Repository.Common.Core;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Repository.Common.Repositories
{
    public interface IBookRepository : IGenericRepository<IBook>
    {
        Task<IPagedList<IBook>> FindBooksAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);

        Task<IPagedList<IBook>> FindByBookIdAsync(Guid bookstoreId, string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize);
    }
}