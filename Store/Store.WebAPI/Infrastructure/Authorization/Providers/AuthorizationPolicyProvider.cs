using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

using Store.Common.Enums;
using Store.WebAPI.Infrastructure.Authorization.Requirements;

namespace Store.WebAPI.Infrastructure.Authorization.Providers
{
    public class AuthorizationPolicyProvider : IAuthorizationPolicyProvider {
        private const string SectionPolicyPrefix = "Section_";

        private DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

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
            //options.Value.AddPolicy("PolicyName", policy => {
            //    policy.Requirements.Add(new Requirement());
            //});

            options.Value.AddPolicy
            (
                "ClientAuthentication",
                new AuthorizationPolicyBuilder("ClientAuthenticationScheme").RequireAuthenticatedUser().Build()
            );

            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName) 
        {
            if (policyName.StartsWith(SectionPolicyPrefix, StringComparison.OrdinalIgnoreCase)) 
            {
                string[] policyData = policyName[SectionPolicyPrefix.Length..].Split('.');

                SectionType sectionType = Enum.Parse<SectionType>(policyData[0]);
                AccessType accessAction = Enum.Parse<AccessType>(policyData[1]);

                AuthorizationPolicyBuilder policy = new();
                policy.AddRequirements(new SectionPolicyRequirement(sectionType, accessAction));

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