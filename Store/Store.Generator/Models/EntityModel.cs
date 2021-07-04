using DotLiquid;
using System.Collections.Generic;

using Store.Common.Extensions;

namespace Store.Generator.Models
{
    internal class EntityModel : Drop 
    {
        public IEnumerable<PropertyModel> Properties { get; init; }

        public string ClassNamespace { get; init; }
        
        public string TableName { get; init; }
        
        public string ClassName { get; init; }
    }
    
    internal class PropertyModel : Drop
    {
        public string SnakeName => Name.ToSnakeCase();

        public string Name { get; init; }
    }
}