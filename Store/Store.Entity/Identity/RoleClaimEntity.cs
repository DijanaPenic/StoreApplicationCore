using System;

namespace Store.Entities.Identity
{
    public class RoleClaimEntity : IDBBaseEntity, IDbChangeable
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public RoleEntity Role { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}