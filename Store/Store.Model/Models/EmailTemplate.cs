using System;

using Store.Common.Enums;
using Store.Model.Common.Models;

namespace Store.Models
{
    public class EmailTemplate : IEmailTemplate
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public Guid ClientId { get; set; }

        public EmailTemplateType Type { get; set; }

        public string Path { get; set; }
    }
}