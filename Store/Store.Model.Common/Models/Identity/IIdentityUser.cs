using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Store.Model.Common.Models.Identity
{
    public interface IIdentityUser : IUser<Guid>
    {
        // Need to override Id from the IUser so that AutoMapper can map the Id property (we need setter)
        new Guid Id { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        string PasswordHash { get; set; }

        string SecurityStamp { get; set; }

        bool IsDeleted { get; set; }

        bool IsApproved { get; set; }

        bool LockoutEnabled { get; set; }

        int AccessFailedCount { get; set; }

        DateTime? LockoutEndDateUtc { get; set; }

        ICollection<IClaim> Claims { get; set; }

        ICollection<IExternalLogin> Logins { get; set; }

        ICollection<IIdentityRole> Roles { get; set; }

        DateTime DateCreatedUtc { get; set; }

        DateTime DateUpdatedUtc { get; set; }
    }
}
