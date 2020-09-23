using X.PagedList;
using System.Threading.Tasks;

using Store.Model.Common.Models;
using Store.Repository.Common.Core;

namespace Store.Repository.Common.Repositories
{
    public interface IBookRepository : IGenericRepository<IBook>
    {
        Task<IPagedList<IBook>> FindAsync(string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize, params string[] includeProperties);
    }
}