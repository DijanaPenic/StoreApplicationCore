﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Repositories.Identity.Stores;

namespace Store.Services.Identity
{
    public sealed class ApplicationRoleManager : RoleManager<IRole>
    {
        private readonly IApplicationRoleStore _roleStore;

        public ApplicationRoleManager(
            IRoleStore<IRole> roleStore,
            IEnumerable<IRoleValidator<IRole>> roleValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            ILogger<ApplicationRoleManager> logger) 
            : base(roleStore, roleValidators, keyNormalizer, errors, logger)
        {
            _roleStore = (IApplicationRoleStore)roleStore;
        }

        public async Task<bool> IsValidRoleSelectionAsync(string[] roles)
        {
            IEnumerable<IRole> selectedRoles = await _roleStore.FindByNameAsync(roles.Select(r => r.ToUpperInvariant()).ToArray(), CancellationToken);
            if(selectedRoles.Count() != roles.Length)
            {
                return false;
            }

            // Unstackable role cannot be combined with other roles
            if(selectedRoles.Any(r => !r.Stackable) && selectedRoles.Count() > 1)
            {
                return false;
            }

            return true;
        }

        public Task<int> GetUserCountByRoleAsync(IRole role)
        {
            return _roleStore.GetUserCountByRoleNameAsync(role.NormalizedName, CancellationToken);
        }

        public Task<int> GetUserRoleCombinationCountAsync(IRole role)
        {
            return _roleStore.GetUserRoleCombinationCountByRoleNameAsync(role.NormalizedName, CancellationToken);
        }

        public Task<IRole> FindRoleByIdAsync(Guid id, params string[] includeProperties)
        {
            if (GuidHelper.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _roleStore.FindRoleByIdAsync(id, CancellationToken, includeProperties);
        }

        public Task<IPagedEnumerable<IRole>> FindRolesAsync(string searchString, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, params string[] includeProperties)
        {
            if (sortOrderProperty == null)
            {
                throw new ArgumentNullException(nameof(sortOrderProperty));
            }

            return _roleStore.FindRolesAsync(searchString, sortOrderProperty, isDescendingSortOrder, pageNumber, pageSize, CancellationToken, includeProperties);
        }

        public Task<IPagedEnumerable<IRole>> FindRolesWithPoliciesAsync(SectionType sectionType, string searchString, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize)
        {
            if (sortOrderProperty == null)
            {
                throw new ArgumentNullException(nameof(sortOrderProperty));
            }

            return _roleStore.FindRolesAndPoliciesAsync(sectionType, searchString, sortOrderProperty, isDescendingSortOrder, pageNumber, pageSize, CancellationToken);
        }

        public async Task<IdentityResult> RemoveClaimsAsync(IRole role, string type, string searchString)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            IApplicationRoleClaimStore claimStore = GetClaimStore();
            await claimStore.RemoveClaimsAsync(role, type, searchString, CancellationToken);

            return await UpdateRoleAsync(role);
        }

        public Task<IPagedEnumerable<IRoleClaim>> FindClaimsAsync(string type, string searchString, bool isDescendingSortOrder, int pageNumber, int pageSize, Guid? roleId = null)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException(nameof(type));
            }

            IApplicationRoleClaimStore claimStore = GetClaimStore();

            return claimStore.FindClaimsAsync(type, searchString, isDescendingSortOrder, pageNumber, pageSize, roleId, CancellationToken);
        }

        private IApplicationRoleClaimStore GetClaimStore()
        {
            if (Store is not IApplicationRoleClaimStore cast)
            {
                throw new NotSupportedException("Store not IApplicationRoleClaimStore.");
            }

            return cast;
        }
    }
}