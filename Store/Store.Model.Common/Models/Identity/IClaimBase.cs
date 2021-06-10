using System;

using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    public interface IClaimBase : IBaseEntity, IChangable
    {
        Guid Id { get; set; }

        string ClaimType { get; set; }

        string ClaimValue { get; set; }
    }
}