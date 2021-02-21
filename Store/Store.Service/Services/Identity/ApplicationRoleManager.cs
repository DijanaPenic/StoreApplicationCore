using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

using Store.Common.Enums;
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

        public Task<IPagedEnumerable<IRole>> FindRolesAsync(string searchString, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize)
        {
            if (sortOrderProperty == null)
            {
                throw new ArgumentNullException(nameof(sortOrderProperty));
            }

            return _roleStore.FindRolesAsync(searchString, sortOrderProperty, isDescendingSortOrder, pageNumber, pageSize);
        }

        public async Task<IdentityResult> RemoveClaimsAsync(IRole role, string type, string valueExpression)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            IApplicationRoleClaimStore claimStore = GetClaimStore();
            await claimStore.RemoveClaimsAsync(role, type, valueExpression, CancellationToken);

            return await UpdateRoleAsync(role);
        }

        private IApplicationRoleClaimStore GetClaimStore()
        {
            IApplicationRoleClaimStore cast = Store as IApplicationRoleClaimStore;
            if (cast == null)
            {
                throw new NotSupportedException("Store not IApplicationRoleClaimStore.");
            }

            return cast;
        }
    }
}