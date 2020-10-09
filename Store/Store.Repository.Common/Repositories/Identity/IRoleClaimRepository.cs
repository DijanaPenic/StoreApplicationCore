using System;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IRoleClaimRepository : IDapperGenericRepository<IRoleClaim, Guid>
    {
        IEnumerable<IRoleClaim> FindByRoleId(string roleId);
    }
}