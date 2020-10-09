using System;

using Store.Model.Common.Models.Identity;

namespace Store.Model.Models.Identity
{
    public class UserLogin : UserLoginKey, IUserLogin
    {
        public string ProviderDisplayName { get; set; }

        public Guid UserId { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }

    public class UserLoginKey : IUserLoginKey
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}