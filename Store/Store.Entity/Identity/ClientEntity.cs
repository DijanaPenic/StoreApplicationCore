using System;
using System.Collections.Generic;

using Store.Common.Enums;

namespace Store.Entities.Identity
{
    public class ClientEntity : IDBBaseEntity, IDBChangable
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Secret { get; set; }

        public string Description { get; set; }

        public ApplicationType ApplicationType { get; set; }

        public bool Active { get; set; }

        public int AccessTokenLifeTime { get; set; }

        public int RefreshTokenLifeTime { get; set; }

        public string AllowedOrigin { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public ICollection<UserRefreshTokenEntity> RefreshTokens { get; set; }

        public ICollection<EmailTemplateEntity> EmailTemplates { get; set; }
    }
}