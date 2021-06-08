using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;
using Microsoft.AspNetCore.Identity;

using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IApplicationRoleStore : IRoleStore<IRole>, IDisposable
    {
        Task<int> GetCountByRoleNameAsync(string normalizedRoleName, CancellationToken cancellationToken);

        Task<IEnumerable<IRole>> FindByNameAsync(string[] normalizedRoleNames, CancellationToken cancellationToken);

        Task<IRole> FindRoleByIdAsync(Guid id, IOptionsParameters options, CancellationToken cancellationToken);

        Task<IPagedList<IRole>> FindRolesAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options, CancellationToken cancellationToken);

        Task<IPagedList<IRole>> FindRolesBySectionAsync(IPermissionFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, CancellationToken cancellationToken);
    }
}