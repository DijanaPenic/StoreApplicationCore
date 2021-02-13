using Microsoft.AspNetCore.Authorization;

namespace Store.WebAPI.Infrastructure.Authorization.Requirements
{
    public class ResourcePermissionRequirement : IAuthorizationRequirement
    {
        public ResourcePermission Permission { get; set; }

        public ResourcePermissionRequirement(ResourcePermission permission)
        {
            Permission = permission;
        }
    } 
}