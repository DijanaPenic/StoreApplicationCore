using Microsoft.AspNetCore.Authorization;

namespace Store.WebAPI.Infrastructure.Attributes
{
    public class AuthorizationFilterAttribute : AuthorizeAttribute
    {
        public AuthorizationFilterAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
    }
}