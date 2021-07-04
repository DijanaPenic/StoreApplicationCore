using DotLiquid;

using Store.Common.Extensions;

namespace Store.Generator.Models
{
    internal class SqlQueryModel : FileModel 
    {
        public SqlParameterModel[] Parameters { get; init; }
    }
    
    internal class SqlParameterModel : Drop
    {
        public string PascalName => Name.ToPascalCase();

        public string Name { get; init; }
    }
}