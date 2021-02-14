using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IApplicationRoleStore<TRole> : IRoleStore<TRole>, IDisposable where TRole : class, IRole
    {
        Task<int> GetUserCountByRoleNameAsync(string normalizedRoleName, CancellationToken cancellationToken);

        Task<int> GetUserRoleCombinationCountByRoleNameAsync(string normalizedRoleName, CancellationToken cancellationToken);

        Task<IEnumerable<IRole>> FindByNameAsync(string[] normalizedRoleNames, CancellationToken cancellationToken);
    }
}