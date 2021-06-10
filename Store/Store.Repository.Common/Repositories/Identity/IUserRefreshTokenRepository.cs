using System.Threading.Tasks;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserRefreshTokenRepository : IRepository<IUserRefreshToken, IUserRefreshTokenKey>
    {
        Task DeleteExpiredAsync();

        Task<IUserRefreshToken> FindByValueAsync(string value);
    }
}