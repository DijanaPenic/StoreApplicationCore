using System;

namespace Store.WebAPI.Models.Identity
{
    public class RoleClaimGetApiModel
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}