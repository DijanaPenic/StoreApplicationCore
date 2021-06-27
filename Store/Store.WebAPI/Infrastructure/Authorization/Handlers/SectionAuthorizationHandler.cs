using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

using Store.Common.Enums;
using Store.Services.Identity;
using Store.Model.Common.Models.Identity;
using Store.WebAPI.Infrastructure.Authorization.Requirements;

namespace Store.WebAPI.Infrastructure.Authorization.Handlers
{
    public class SectionAuthorizationHandler : AuthorizationHandler<SectionPolicyRequirement>
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationPermissionsManager _permissionsManager;

        public SectionAuthorizationHandler(ApplicationUserManager userManager, ApplicationPermissionsManager permissionsManager)
        {
            _userManager = userManager;
            _permissionsManager = permissionsManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SectionPolicyRequirement requirement)
        {
            IUser user = await _userManager.GetUserAsync(context.User);
            if (user == null)
            {
                return;
            }

            IList<Claim> roleClaims = await _permissionsManager.BuildRoleClaimsAsync(user);

            bool RoleClaimPredicate(Claim rc)
            {
                string[] sectionData = rc.Value.Split('.');

                return Enum.Parse<SectionType>(sectionData[0]) == requirement.SectionType && 
                       Enum.Parse<AccessType>(sectionData[1]) == requirement.AccessAction;
            }

            if (roleClaims.FirstOrDefault(RoleClaimPredicate) != null)
            {
                context.Succeed(requirement);
            }
        }
    }
}