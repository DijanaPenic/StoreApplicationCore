﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IApplicationRoleStore : IRoleStore<IRole>, IDisposable
    {
        Task<int> GetCountByRoleNameAsync(string normalizedRoleName, CancellationToken cancellationToken);

        Task<IEnumerable<IRole>> FindByNameAsync(string[] normalizedRoleNames, CancellationToken cancellationToken);

        Task<IRole> FindRoleByIdAsync(Guid id, IOptionsParameters options, CancellationToken cancellationToken);

        Task<IPagedEnumerable<IRole>> FindRolesAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options, CancellationToken cancellationToken);

        Task<IPagedEnumerable<IRole>> FindRolesAndPoliciesAsync(IPermissionFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, CancellationToken cancellationToken);
    }
}