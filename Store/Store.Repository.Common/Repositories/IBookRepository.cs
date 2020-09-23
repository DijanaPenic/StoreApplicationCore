using X.PagedList;
using System.Threading.Tasks;

using Store.Model.Common.Models;

namespace Store.Repository.Common.Repositories
{
    public interface IBookRepository
    {
        Task<IPagedList<IBook>> FindAsync(string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize, params string[] includeProperties);
    }
}