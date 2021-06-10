using System;

using Store.Common.Enums;
using Store.Entities.Identity;

namespace Store.Entities
{
    public class EmailTemplateEntity : IDBBaseEntity, IDBChangable
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public Guid ClientId { get; set; }

        public ClientEntity Client { get; set; }

        public EmailTemplateType Type { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }
    }
}