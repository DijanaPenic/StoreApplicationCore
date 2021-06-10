using System;

using Store.Common.Enums;
using Store.Model.Common.Models.Core;
using Store.Model.Common.Models.Identity;

namespace Store.Model.Common.Models
{
    public interface IEmailTemplate : IBaseEntity, IChangable
    {
        Guid Id { get; set; }

        Guid ClientId { get; set; }

        IClient Client { get; set; }

        EmailTemplateType Type { get; set; }

        string Path { get; set; }

        string Name { get; set; }
    }
}