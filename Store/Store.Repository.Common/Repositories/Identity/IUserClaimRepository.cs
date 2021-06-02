using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserClaimRepository : IIdentityRepository<IUserClaim, Guid>
    {
        Task<IEnumerable<IUserClaim>> GetByUserIdAsync(Guid userId);

        Task<IEnumerable<IUser>> GetUsersForClaimAsync(string claimType, string claimValue);
    }
}