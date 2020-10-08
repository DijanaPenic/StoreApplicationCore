using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    public interface IClaimBase : IPoco
    {
        string ClaimType { get; set; }

        string ClaimValue { get; set; }
    }
}