using System.Security.Claims;
using System.Collections.Generic;

namespace Store.Services.Identity
{
    internal class ClaimsComparer : IEqualityComparer<Claim>
    {
        public bool Equals(Claim x, Claim y)
        {
            return x.Value == y.Value;
        }

        public int GetHashCode(Claim claim)
        {
            return claim.Value?.GetHashCode() ?? 0;
        }
    }
}