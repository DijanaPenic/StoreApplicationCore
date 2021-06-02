using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserClaimRepository : IIdentityRepository<IUserClaim, Guid>
    {
        Task<IEnumerable<IUserClaim>> FindByUserIdAsync(Guid userId);

        Task<IEnumerable<IUser>> FindUsersByClaimAsync(string claimType, string claimValue);
    }
}