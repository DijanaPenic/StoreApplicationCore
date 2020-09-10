using System;

using Store.Model.Common.Models.Identity;

namespace Store.Models.Identity
{
    public class Claim : IClaim
    {
        public int Id { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public Guid UserId { get; set; }

        public IIdentityUser User { get; set; }
    }
}