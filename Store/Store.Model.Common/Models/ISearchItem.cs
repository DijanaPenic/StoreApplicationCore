using System;

using Store.Common.Enums;

namespace Store.Model.Common.Models
{
    public interface ISearchItem
    {
        Guid Id { get; set; }

        string Name { get; set; }

        SectionType Type { get; set; }
    }
}