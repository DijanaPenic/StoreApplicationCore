using System;

using Store.Model.Common.Models.Identity;

namespace Store.Model.Models.Identity
{
    public class UserClaim : ClaimBase, IUserClaim
    {
        public Guid UserId { get; set; }
    }
}