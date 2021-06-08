using System;

namespace Store.Entities.Identity
{
    public class UserRoleEntity 
    {
        public Guid UserId { get; set; }

        public UserEntity User { get; set; }

        public Guid RoleId { get; set; }

        public RoleEntity Role { get; set; }

        public DateTime DateCreatedUtc { get; set; }
    }
}