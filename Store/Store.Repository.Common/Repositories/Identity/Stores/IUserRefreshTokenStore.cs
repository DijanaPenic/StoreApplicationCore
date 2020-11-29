using System;
using System.Threading.Tasks;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IUserRefreshTokenStore
    {
        Task AddRefreshTokenAsync(IUserRefreshToken refreshToken);

        Task RemoveRefreshTokenAsync(Guid refreshTokenId);

        Task RemoveRefreshTokenAsync(Guid userId, Guid clientId);

        Task RemoveExpiredRefreshTokensAsync();

        Task<IUserRefreshToken> FindRefreshTokenByIdAsync(Guid refreshTokenId);

        Task<IUserRefreshToken> FindRefreshTokenByValueAsync(string refreshTokenValue);
    }
}