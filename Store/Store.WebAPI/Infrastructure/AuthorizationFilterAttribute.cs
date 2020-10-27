using Microsoft.AspNetCore.Authorization;

namespace Store.WebAPI.Infrastructure
{
    public class AuthorizationFilterAttribute : AuthorizeAttribute
    {
        public AuthorizationFilterAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
    }
}