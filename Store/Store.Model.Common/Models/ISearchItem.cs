using System;

using Store.Common.Enums;

namespace Store.Model.Common.Models
{
    public interface ISearchItem
    {
        Guid Id { get; set; }

        string Name { get; set; }

        ModuleType Type { get; set; }
    }
}