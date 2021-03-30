using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
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

        public Task<IRole> FindRoleByIdAsync(Guid id, IOptionsParameters options)
        {
            if (GuidHelper.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _roleStore.FindRoleByIdAsync(id, options, CancellationToken);
        }

        public Task<IPagedEnumerable<IRole>> FindRolesAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            return _roleStore.FindRolesAsync(filter, paging, sorting, options, CancellationToken);
        }

        public Task<IPagedEnumerable<IRole>> FindRolesWithPoliciesAsync(IPermissionFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting)
        {
            return _roleStore.FindRolesAndPoliciesAsync(filter, paging, sorting, CancellationToken);
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

            return claimStore.FindClaimsAsync(type, searchString, isDescendingSortOrder, pageNumber, pageSize, CancellationToken, roleId);
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