using System;

namespace Store.Entities.Identity
{
    public class UserRefreshTokenEntity : IDBPoco
    {
        public Guid Id { get; set; }

        public string Value { get; set; }

        public Guid UserId { get; set; }

        public Guid ClientId { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public DateTime ExpiresUtc { get; set; }
    }
}