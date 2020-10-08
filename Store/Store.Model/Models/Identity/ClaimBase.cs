using System;

using Store.Model.Common.Models.Identity;

namespace Store.Model.Models.Identity
{
    public abstract class ClaimBase : IClaimBase
    {
        public Guid Id { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}