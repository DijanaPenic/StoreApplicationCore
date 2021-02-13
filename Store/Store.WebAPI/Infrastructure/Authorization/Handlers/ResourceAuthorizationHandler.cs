using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

using Store.Common.Extensions;
using Store.Services.Identity;
using Store.Model.Common.Models.Identity;
using Store.WebAPI.Infrastructure.Authorization.Requirements;

namespace Store.WebAPI.Infrastructure.Authorization.Handlers
{
    public class ResourceAuthorizationHandler : AuthorizationHandler<ResourcePermissionRequirement>
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IPermissionsBuilder _permissionsBuilder;

        public ResourceAuthorizationHandler(ApplicationUserManager userManager, IPermissionsBuilder permissionsBuilder)
        {
            _userManager = userManager;
            _permissionsBuilder = permissionsBuilder;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourcePermissionRequirement requirement)
        {
            if (context.User == null)
            {
                return;
            }

            IUser user = await _userManager.GetUserAsync(context.User);
            if (user == null)
            {
                return;
            }

            IList<Claim> roleClaims = await _permissionsBuilder.BuildRoleClaims(user);

            if (roleClaims.FirstOrDefault(c => c.Value == requirement.Permission.GetEnumDescription()) != null)
            {
                context.Succeed(requirement);
            }
        }
    }
}