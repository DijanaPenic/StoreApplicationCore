using System.Linq;
using System.Security.Principal;

namespace Store.Common.Extensions
{
    public static class PrincipalExtensions
    {

        public static bool IsInRoles(this IPrincipal source, params string[] roles)
        {
            return roles.Any(role => source.IsInRole(role));
        }
    }
}