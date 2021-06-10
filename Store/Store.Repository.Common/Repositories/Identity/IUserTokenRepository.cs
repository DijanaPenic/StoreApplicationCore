using X.PagedList;
using System.Threading.Tasks;

using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserTokenRepository : IRepository<IUserToken, IUserTokenKey>
    {
        Task<IPagedList<IUserToken>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null);
    }
}