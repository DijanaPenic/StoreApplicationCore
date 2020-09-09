using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Entities.Identity
{
    public class UserRoleEntity
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public UserEntity User { get; set; }

        [ForeignKey("Role")]
        public Guid RoleId { get; set; }

        public RoleEntity Role { get; set; }
    }
}
