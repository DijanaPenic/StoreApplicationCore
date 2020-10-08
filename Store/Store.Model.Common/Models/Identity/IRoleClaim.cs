using System;

namespace Store.Model.Common.Models.Identity
{
    public interface IRoleClaim : IClaimBase
    {
        Guid RoleId { get; set; }
    }
}