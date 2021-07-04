using DotLiquid;

namespace Store.Generator.Models
{
    internal class FileModel : Drop
    {
        public string Name { get; init; }

        public string RelativePath { get; init; }

        public string FullPath { get; init; }
    }
}