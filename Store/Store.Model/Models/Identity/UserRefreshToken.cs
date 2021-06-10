using System;

using Store.Model.Common.Models.Identity;

namespace Store.Models.Identity
{
    public class UserRefreshToken : IUserRefreshToken
    {
        public Guid Id { get; set; }

        public string Value { get; set; }

        public Guid UserId { get; set; }

        public Guid ClientId { get; set; }

        public IClient Client { get; set; }

        public DateTime DateExpiresUtc { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}
