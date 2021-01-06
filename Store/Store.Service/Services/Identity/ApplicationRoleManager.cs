using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Repositories.Identity.Stores;

namespace Store.Services.Identity
{
    public sealed class ApplicationRoleManager : RoleManager<IRole>
    {
        private readonly IApplicationRoleStore<IRole> _roleStore;

        public ApplicationRoleManager(
            IRoleStore<IRole> roleStore,
            IEnumerable<IRoleValidator<IRole>> roleValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            ILogger<ApplicationRoleManager> logger) 
            : base(roleStore, roleValidators, keyNormalizer, errors, logger)
        {
            _roleStore = (IApplicationRoleStore<IRole>)roleStore;
        }

        public async Task<bool> IsValidRoleSelectionAsync(string[] roles)
        {
            IEnumerable<IRole> systemRoles = await GetRolesAsync();

            // Unstackable role cannot be combined with other roles
            IEnumerable<IRole> selectedRoles = systemRoles.Where(r => roles.Contains(r.Name));

            if (roles.Except(systemRoles.Select(r => r.Name)).Any() || (selectedRoles.Any(r => !r.Stackable) && selectedRoles.Count() > 1))
            return false;

            return true;
        }

        public Task<IEnumerable<IRole>> GetRolesAsync()
        {
            return _roleStore.GetRolesAsync();
        }
    }
}