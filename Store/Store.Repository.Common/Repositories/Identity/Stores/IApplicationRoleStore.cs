﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using Store.Common.Enums;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IApplicationRoleStore : IRoleStore<IRole>, IDisposable
    {
        Task<int> GetUserCountByRoleNameAsync(string normalizedRoleName, CancellationToken cancellationToken);

        Task<int> GetUserRoleCombinationCountByRoleNameAsync(string normalizedRoleName, CancellationToken cancellationToken);

        Task<IEnumerable<IRole>> FindByNameAsync(string[] normalizedRoleNames, CancellationToken cancellationToken);

        Task<IRole> FindRoleByIdAsync(Guid id, CancellationToken cancellationToken, params string[] includeProperties);

        Task<IPagedEnumerable<IRole>> FindRolesAsync(string searchString, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, CancellationToken cancellationToken, params string[] includeProperties);

        Task<IPagedEnumerable<IRole>> FindRolesAndPoliciesAsync(SectionType sectionType, string searchString, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}