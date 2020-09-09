using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.Entities.Identity
{
    public class UserEntity : IDBPoco
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
        public string LastName { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsApproved { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public ICollection<ClaimEntity> Claims { get; set; }

        public ICollection<ExternalLoginEntity> Logins { get; set; }

        public ICollection<UserRoleEntity> Roles { get; set; }
    }
}