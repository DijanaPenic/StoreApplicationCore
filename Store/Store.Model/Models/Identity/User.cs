using System;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;

namespace Store.Model.Models.Identity
{
    public class User : IUser
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string NormalizedEmail { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public bool LockoutEnabled { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsApproved { get; set; }

        public int AccessFailedCount { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string SecurityStamp { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public ICollection<IRole> Roles { get; set; }
    }
}