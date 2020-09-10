using System;

namespace Store.Model.Common.Models.Identity
{
    public interface IClaim 
    {
        int Id { get; set; }

        string ClaimType { get; set; }

        string ClaimValue { get; set; }

        Guid UserId { get; set; }

        IIdentityUser User { get; set; }
    }
}
