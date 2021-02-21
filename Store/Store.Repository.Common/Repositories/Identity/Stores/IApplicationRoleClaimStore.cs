using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Store.Common.Enums;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IApplicationRoleClaimStore : IRoleClaimStore<IRole>, IDisposable
    {
        Task RemoveClaimsAsync(IRole role, string type, string valueExpression, CancellationToken cancellationToken = default);
    }
}