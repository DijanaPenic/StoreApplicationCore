using X.PagedList;
using System.Threading.Tasks;

using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserRefreshTokenRepository : IRepository<IUserRefreshToken, IUserRefreshTokenKey>
    {
        Task DeleteExpiredAsync();

        Task<IUserRefreshToken> FindByValueAsync(string value);

        Task<IPagedList<IUserRefreshToken>> FindAsync(IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null);
    }
}