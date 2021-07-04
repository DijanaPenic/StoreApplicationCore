using System;
using System.Collections.Generic;

namespace Store.Entities.Identity
{
    public class RoleEntity : IDbBaseEntity, IDbChangeable
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public string ConcurrencyStamp { get; set; }

        public bool Stackable { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public ICollection<RoleClaimEntity> Claims { get; set; }

        public ICollection<UserEntity> Users { get; set; }
    }
}