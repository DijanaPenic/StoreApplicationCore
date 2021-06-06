using System;
using System.Threading;
using System.Threading.Tasks;
using X.PagedList;
using Microsoft.AspNetCore.Identity;

using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IApplicationRoleClaimStore : IRoleClaimStore<IRole>, IDisposable
    {
        Task RemoveClaimsAsync(IRoleClaimFilteringParameters filter, CancellationToken cancellationToken);

        Task<IPagedList<IRoleClaim>> FindClaimsAsync(IRoleClaimFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, CancellationToken cancellationToken);
    }
}