using System;

using Store.Model.Common.Models.Identity;

namespace Store.Models.Identity
{
    public class UserRefreshToken : UserRefreshTokenKey, IUserRefreshToken
    {
        public string Value { get; set; }

        public IUser User { get; set; }

        public IClient Client { get; set; }

        public DateTime DateExpiresUtc { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }

    public class UserRefreshTokenKey : IUserRefreshTokenKey
    {
        public Guid UserId { get; set; }

        public Guid ClientId { get; set; }

        public object[] ToArray() => new object[] { UserId, ClientId };
    }
}
