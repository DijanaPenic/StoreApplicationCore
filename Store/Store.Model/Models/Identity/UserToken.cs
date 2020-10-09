using System;

using Store.Model.Common.Models.Identity;

namespace Store.Model.Models.Identity
{
    public class UserToken : UserTokenKey, IUserToken
    {
        public string Value { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }

    public class UserTokenKey : IUserTokenKey
    {
        public Guid UserId { get; set; }

        public string LoginProvider { get; set; }

        public string Name { get; set; }
    }
}