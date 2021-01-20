using Microsoft.AspNetCore.Authorization;

namespace Store.WebAPI.Infrastructure.Authorization.Attributes
{
    public class UserAuthorizationAttribute : AuthorizeAttribute
    {
        public UserAuthorizationAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
    }
}