using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Store.Common.Enums;
using Store.WebAPI.Infrastructure.Authorization.Attributes;

namespace Store.WebAPI.Infrastructure.Authorization.Extensions
{
    public static class AuthorizationServiceExtensions
    {
        public static Task<AuthorizationResult> AuthorizeAsync(this IAuthorizationService service, ClaimsPrincipal user, SectionType sectionType, AccessType accessAction)
        {
            SectionAuthorizationAttribute sectionAuthorizationAttribute = new(sectionType, accessAction);
            
            return service.AuthorizeAsync(user, sectionAuthorizationAttribute.Policy ?? throw new InvalidOperationException());
        }
    }
}