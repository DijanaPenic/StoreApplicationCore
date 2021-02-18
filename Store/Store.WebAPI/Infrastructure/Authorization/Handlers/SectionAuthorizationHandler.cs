using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

using Store.Services.Identity;
using Store.Model.Common.Models.Identity;
using Store.WebAPI.Infrastructure.Authorization.Requirements;

namespace Store.WebAPI.Infrastructure.Authorization.Handlers
{
    public class SectionAuthorizationHandler : AuthorizationHandler<SectionPolicyRequirement>
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IPermissionsBuilder _permissionsBuilder;

        public SectionAuthorizationHandler(ApplicationUserManager userManager, IPermissionsBuilder permissionsBuilder)
        {
            _userManager = userManager;
            _permissionsBuilder = permissionsBuilder;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SectionPolicyRequirement requirement)
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

            bool roleClaimPredicate(Claim rc)
            {
                string[] sectionData = rc.Value.Split('.');

                return Enum.Parse<SectionType>(sectionData[0]) == requirement.SectionType && Enum.Parse<AccessAction>(sectionData[1]) == requirement.AccessAction;
            }

            if (roleClaims.FirstOrDefault(roleClaimPredicate) != null)
            {
                context.Succeed(requirement);
            }
        }
    }
}