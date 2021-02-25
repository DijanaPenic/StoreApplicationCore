using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IApplicationRoleClaimStore : IRoleClaimStore<IRole>, IDisposable
    {
        Task RemoveClaimsAsync(IRole role, string type, string searchString, CancellationToken cancellationToken = default);

        Task<IPagedEnumerable<IRoleClaim>> FindClaimsAsync(string type, string searchString, bool isDescendingSortOrder, int pageNumber, int pageSize, Guid? roleId = null, CancellationToken cancellationToken = default);
    }
}