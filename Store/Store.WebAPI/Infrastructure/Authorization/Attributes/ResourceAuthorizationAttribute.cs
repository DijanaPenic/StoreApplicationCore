using System;
using Microsoft.AspNetCore.Authorization;

namespace Store.WebAPI.Infrastructure.Authorization.Attributes
{
    public class ResourceAuthorizationAttribute : AuthorizeAttribute 
    {
        const string POLICY_PREFIX = "Resource_";

        public ResourceAuthorizationAttribute(ResourcePermission resourcePermission) => ResourcePermission = resourcePermission;

        // Get or set the ResourcePermission property by manipulating the underlying Policy property
        public ResourcePermission ResourcePermission
        {
            get 
            {
                return (ResourcePermission)Enum.Parse(typeof(ResourcePermission), Policy[POLICY_PREFIX.Length..]);
            }
            set 
            {
                Policy = $"{POLICY_PREFIX}{value}";
            }
        }
    }
}