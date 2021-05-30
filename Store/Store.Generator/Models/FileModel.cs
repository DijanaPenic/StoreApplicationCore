using DotLiquid;

namespace Store.Generator.Models
{
    internal class FileModel : Drop
    {
        public string Name { get; set; }

        public string RelativePath { get; set; }

        public string FullPath { get; set; }
    }
}