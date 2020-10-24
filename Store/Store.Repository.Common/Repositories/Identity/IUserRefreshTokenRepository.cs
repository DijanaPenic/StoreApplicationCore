using System;
using System.Threading.Tasks;

using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserRefreshTokenRepository : IDapperGenericRepository<IUserRefreshToken, Guid>
    {
        Task DeleteExpiredAsync();

        Task<IUserRefreshToken> FindByValueAsync(string value);
    }
}