using System.Threading.Tasks;

using Store.Common.Parameters.Options;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IUserRefreshTokenStore
    {
        Task AddRefreshTokenAsync(IUserRefreshToken refreshToken);

        Task RemoveRefreshTokenByKeyAsync(IUserRefreshTokenKey key);

        Task RemoveExpiredRefreshTokensAsync();

        Task<IUserRefreshToken> FindRefreshTokenByKeyAsync(IUserRefreshTokenKey key, IOptionsParameters options);

        Task<IUserRefreshToken> FindRefreshTokenByValueAsync(string refreshTokenValue);
    }
}