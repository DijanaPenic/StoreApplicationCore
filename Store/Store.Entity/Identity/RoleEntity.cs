using System;
using System.Collections.Generic;

namespace Store.Entities.Identity
{
    public class RoleEntity : IDBPoco
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Stackable { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public ICollection<UserRoleEntity> Users { get; set; }
    }
}