using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserLoginRepository : IDapperGenericRepository<IUserLogin, IUserLoginKey>
    {
        Task<IEnumerable<IUserLogin>> FindByUserIdAsync(Guid userId);

        Task<IUserLogin> FindAsync(IUserLoginKey key, bool isConfirmed);

        Task<IUserLogin> FindAsync(Guid userId, string token);
    }
}