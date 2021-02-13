using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

using Store.WebAPI.Infrastructure.Authorization.Requirements;

namespace Store.WebAPI.Infrastructure.Authorization.Providers
{
    public class AuthorizationPolicyProvider : IAuthorizationPolicyProvider {
        const string RESOURCE_POLICY_PREFIX = "Resource_";

        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) {
            // ASP.NET Core only uses one authorization policy provider, so if the custom implementation
            // doesn't handle all policies (including default policies, etc.) it should fall back to an
            // alternate provider.
            //
            // In this sample, a default authorization policy provider (constructed with options from the 
            // dependency injection container) is used if this custom provider isn't able to handle a given
            // policy name.

            // Claims based authorization
            //options.Value.AddPolicy("PolicyName", policy => {
            //    policy.RequireClaim("ClaimName");
            //});

            // Role based authorization
            //options.Value.AddPolicy("PolicyName", policy => {
            //    policy.RequireRole("RoleName");
            //});

            // Requirement authorization
            //options.Value.AddPolicy("PolicyName", policy =>
            //    {
            //        policy.Requirements.Add(new Requirement());
            //    }
            //); 

            options.Value.AddPolicy("ClientAuthentication", new AuthorizationPolicyBuilder("ClientAuthenticationScheme").RequireAuthenticatedUser().Build());

            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName) 
        {
            if (policyName.StartsWith(RESOURCE_POLICY_PREFIX, StringComparison.OrdinalIgnoreCase)) 
            {
                ResourcePermission resourcePermission = (ResourcePermission)Enum.Parse(typeof(ResourcePermission), policyName[RESOURCE_POLICY_PREFIX.Length..]);

                AuthorizationPolicyBuilder policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new ResourcePermissionRequirement(resourcePermission));

                return Task.FromResult(policy.Build());
            } 
            else 
            {
                return FallbackPolicyProvider.GetPolicyAsync(policyName);
            }
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();
    }
}