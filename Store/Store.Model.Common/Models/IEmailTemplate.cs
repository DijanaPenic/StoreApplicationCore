using System;

using Store.Common.Enums;
using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models
{
    public interface IEmailTemplate : IPoco
    {
        Guid ClientId { get; set; }

        EmailTemplateType Type { get; set; }

        string Path { get; set; }
        string Name { get; set; }
    }
}