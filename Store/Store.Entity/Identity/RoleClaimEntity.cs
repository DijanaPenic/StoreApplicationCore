using System;

namespace Store.Entities.Identity
{
    public class RoleClaimEntity : IDBPoco
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}