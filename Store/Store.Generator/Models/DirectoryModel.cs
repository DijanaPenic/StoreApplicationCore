using DotLiquid;
using System.Collections.Generic;

namespace Store.Generator.Models
{
    internal class DirectoryModel : Drop
    {
        public string Name { get; set; }

        public IList<FileModel> Files { get; set; }
    }
}