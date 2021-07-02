using System;

namespace Store.Entities.Identity
{
    public class UserLoginEntity : IDBBaseEntity, IDbChangeable
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public string ProviderDisplayName { get; set; }

        public string Token { get; set; }

        public bool IsConfirmed { get; set; }

        public Guid UserId { get; set; }

        public UserEntity User { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }
    }
}