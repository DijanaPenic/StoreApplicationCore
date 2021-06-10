using System;

namespace Store.Entities.Identity
{
    public class UserClaimEntity : IDBBaseEntity, IDBChangable
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public UserEntity User { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}