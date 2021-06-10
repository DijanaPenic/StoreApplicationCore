using System.Threading.Tasks;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserRefreshTokenRepository : IIdentityRepository<IUserRefreshToken, IUserRefreshTokenKey>
    {
        Task DeleteExpiredAsync();

        Task<IUserRefreshToken> FindByValueAsync(string value);
    }
}