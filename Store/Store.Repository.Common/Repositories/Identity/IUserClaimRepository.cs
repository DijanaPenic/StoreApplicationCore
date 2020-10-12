using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserClaimRepository : IDapperGenericRepository<IUserClaim, Guid>
    {
        Task<IEnumerable<IUserClaim>> GetByUserIdAsync(Guid userId);

        Task<IEnumerable<IUser>> GetUsersForClaimAsync(string claimType, string claimValue);
    }
}