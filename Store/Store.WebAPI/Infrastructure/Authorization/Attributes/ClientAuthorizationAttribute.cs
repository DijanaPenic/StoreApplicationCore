using Microsoft.AspNetCore.Authorization;

namespace Store.WebAPI.Infrastructure.Authorization.Attributes
{
    public class ClientAuthorizationAttribute : AuthorizeAttribute
    {
        public ClientAuthorizationAttribute()
        {
            Policy = "ClientAuthenticationPolicy";
        }
    }
}