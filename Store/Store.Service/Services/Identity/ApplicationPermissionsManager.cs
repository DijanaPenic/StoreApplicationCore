using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models.Identity;

namespace Store.Services.Identity
{
    public class ApplicationPermissionsManager
    {
        public const string CLAIM_KEY = "Permission";

        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly IFilteringFactory _filteringFactory;

        public ApplicationPermissionsManager
        (
            ApplicationUserManager userManager, 
            ApplicationRoleManager roleManager, 
            IFilteringFactory filteringFactory
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _filteringFactory = filteringFactory;
        }

        public async Task<IList<Claim>> BuildRoleClaimsAsync(IUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            List<Claim> userRoleClaims = new List<Claim>();

            if (_userManager.SupportsUserRole)
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);

                foreach (string roleName in roles)
                {
                    if (_roleManager.SupportsRoleClaims)
                    {
                        IRole role = await _roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            IList<Claim> roleClaims = await _roleManager.GetClaimsAsync(role);
                            userRoleClaims.AddRange(roleClaims.ToList());
                        }
                    }

                    userRoleClaims = userRoleClaims.Distinct(new ClaimsComparer()).ToList();
                }
            }

            return userRoleClaims;
        }

        public async Task<IdentityResult> UpdatePolicyAsync(IRole role, IPolicy policy)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            try
            {
                if (_roleManager.SupportsRoleClaims)
                {
                    IRoleClaimFilteringParameters filter = _filteringFactory.Create<IRoleClaimFilteringParameters>(searchString: $"{policy.Section}.");
                    filter.RoleId = role.Id;
                    filter.Type = CLAIM_KEY;

                    // Delete all role claims for section
                    await _roleManager.RemoveClaimsAsync(role, filter);

                    foreach (IAccessAction accessAction in policy.Actions.Where(a => a.IsEnabled))
                    {
                        Claim roleClaim = new Claim(CLAIM_KEY, $"{policy.Section}.{accessAction.Type}");
                        await _roleManager.AddClaimAsync(role, roleClaim);
                    }

                    return IdentityResult.Success;
                }

                return IdentityResult.Failed(new IdentityError { Code = "Not Supported", Description = "The SupportsRoleClaims flag is not enabled." });
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message });
            }
        }
    }
}