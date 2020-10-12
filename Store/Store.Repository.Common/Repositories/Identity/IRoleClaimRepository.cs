using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IRoleClaimRepository : IDapperGenericRepository<IRoleClaim, Guid>
    {
        Task<IEnumerable<IRoleClaim>> FindByRoleIdAsync(Guid roleId);
    }
}