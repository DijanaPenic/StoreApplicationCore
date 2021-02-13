using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;

using Store.Services.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Infrastructure.Authorization
{
    public class PermissionsBuilder : IPermissionsBuilder
    {
        private readonly ApplicationUserManager _userManager;

        private readonly ApplicationRoleManager _roleManager;

        public PermissionsBuilder(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IList<Claim>> BuildRoleClaims(IUser user)
        {
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
    }
}