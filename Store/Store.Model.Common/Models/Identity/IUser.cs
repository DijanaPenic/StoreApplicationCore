using System;
using System.Collections.Generic;

using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    public interface IUser : IPoco
    {
        string UserName { get; set; }

        string NormalizedUserName { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        string Email { get; set; }

        bool EmailConfirmed { get; set; }

        string NormalizedEmail { get; set; }

        string PhoneNumber { get; set; }

        bool PhoneNumberConfirmed { get; set; }

        string PasswordHash { get; set; }

        bool TwoFactorEnabled { get; set; }

        bool LockoutEnabled { get; set; }

        bool IsApproved { get; set; }

        int AccessFailedCount { get; set; }

        string ConcurrencyStamp { get; set; }

        string SecurityStamp { get; set; }

        DateTime? LockoutEndDateUtc { get; set; }

        ICollection<IRole> Roles { get; set; }

        ICollection<IUserClaim> Claims { get; set; }

        ICollection<IUserLogin> Logins { get; set; }

        ICollection<IUserToken> UserTokens { get; set; }

        ICollection<IUserRefreshToken> RefreshTokens { get; set; }
    }
}
