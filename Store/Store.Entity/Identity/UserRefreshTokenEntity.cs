using System;

namespace Store.Entities.Identity
{
    public class UserRefreshTokenEntity : IDBBaseEntity, IDBChangable
    {
        public Guid Id { get; set; }

        public string Value { get; set; }

        public Guid UserId { get; set; }

        public UserEntity User { get; set; }

        public Guid ClientId { get; set; }

        public ClientEntity Client { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public DateTime DateExpiresUtc { get; set; }
    }
}