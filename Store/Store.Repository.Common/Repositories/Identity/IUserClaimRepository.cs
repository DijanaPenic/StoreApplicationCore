using System;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserClaimRepository : IDapperGenericRepository<IUserClaim, Guid>
    {
        IEnumerable<IUserClaim> GetByUserId(string userId);

        IEnumerable<IUser> GetUsersForClaim(string claimType, string claimValue);
    }
}