using System;
using Microsoft.AspNetCore.Identity;

namespace Store.Entities.Identity
{
    public class UserEntity : IDBPoco
    {
        public Guid Id { get; set; }

        [ProtectedPersonalData]
        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        [PersonalData] 
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }

        [ProtectedPersonalData]
        public string Email { get; set; }

        [PersonalData]
        public bool EmailConfirmed { get; set; }

        public string NormalizedEmail { get; set; }

        [ProtectedPersonalData]
        public string PhoneNumber { get; set; }

        [PersonalData]
        public bool PhoneNumberConfirmed { get; set; }

        public string PasswordHash { get; set; }

        [PersonalData]
        public bool TwoFactorEnabled { get; set; }

        public bool LockoutEnabled { get; set; }

        public bool IsApproved { get; set; }

        public int AccessFailedCount { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string SecurityStamp { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}