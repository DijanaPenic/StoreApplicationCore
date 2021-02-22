using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models.Identity;

namespace Store.Services.Identity
{
    public class ApplicationPermissionsManager
    {
        public const string CLAIM_KEY = "Permission";

        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;

        public ApplicationPermissionsManager(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IList<Claim>> BuildRoleClaimsAsync(IUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            List<Claim> roleClaims = new List<Claim>();

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
                            IList<Claim> rc = await _roleManager.GetClaimsAsync(role);
                            roleClaims.AddRange(rc.ToList());
                        }
                    }

                    roleClaims = roleClaims.Distinct(new ClaimsComparer()).ToList();
                }
            }

            return roleClaims;
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
                    // Delete all role claims for section
                    await _roleManager.RemoveClaimsAsync(role, CLAIM_KEY, $"{policy.Section}.");

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