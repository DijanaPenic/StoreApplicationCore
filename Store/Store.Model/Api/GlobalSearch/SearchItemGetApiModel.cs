using System;

using Store.Common.Enums;

namespace Store.Models.Api.GlobalSearch
{
    public class SearchItemGetApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ModuleType Type { get; set; }
    }
}