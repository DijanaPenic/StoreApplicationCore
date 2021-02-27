using System;

using Store.Common.Enums;
using Store.Model.Common.Models;

namespace Store.Models
{
    public class SearchItem : ISearchItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public SectionType Type { get; set; }
    }
}