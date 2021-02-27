using System;

using Store.Common.Enums;

namespace Store.WebAPI.Models.GlobalSearch
{
    public class SearchItemGetApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public SectionType Type { get; set; }
    }
}