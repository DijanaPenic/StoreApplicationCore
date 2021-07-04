using DotLiquid;
using System.Collections.Generic;

namespace Store.Generator.Models
{
    internal class DirectoryModel : Drop
    {
        public string Name { get; init; }

        public IList<FileModel> Files { get; init; }
    }
}