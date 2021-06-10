using System;
using System.Threading.Tasks;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IUserRefreshTokenStore
    {
        Task AddRefreshTokenAsync(IUserRefreshToken refreshToken);

        Task RemoveRefreshTokenByKeyAsync(IUserRefreshTokenKey key);

        Task RemoveExpiredRefreshTokensAsync();

        Task<IUserRefreshToken> FindRefreshTokenByKeyAsync(IUserRefreshTokenKey key);

        Task<IUserRefreshToken> FindRefreshTokenByValueAsync(string refreshTokenValue);
    }
}