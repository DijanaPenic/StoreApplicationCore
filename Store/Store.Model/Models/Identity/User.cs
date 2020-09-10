using System;
using System.Collections.Generic;

using Store.Common.Helpers;
using Store.Model.Common.Models.Identity;

namespace Store.Models.Identity
{
    public class User : IIdentityUser
    {
        public User()
        {
            Id = GuidHelper.NewSequentialGuid();
        }

        public Guid Id { get; set; }
        
        public string UserName { get; set; }

        public string FirstName { get; set; }

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

        public ICollection<IClaim> Claims { get; set; }

        public ICollection<IExternalLogin> Logins { get; set; }

        public ICollection<IIdentityRole> Roles { get; set; }
    }
}