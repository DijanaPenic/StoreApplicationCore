using DotLiquid;

using Store.Common.Extensions;

namespace Store.Generator.Models
{
    internal class SqlParameterModel : Drop
    {
        public string DisplayName => Name.ToPascalCase();

        public string Name { get; set; }
    }
}