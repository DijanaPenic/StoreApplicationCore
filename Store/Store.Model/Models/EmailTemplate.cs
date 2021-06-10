using System;

using Store.Common.Enums;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;

namespace Store.Models
{
    public class EmailTemplate : IEmailTemplate
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public Guid ClientId { get; set; }

        public IClient Client { get; set; }

        public EmailTemplateType Type { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }
    }
}