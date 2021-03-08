using System.Collections.Generic;

namespace Store.Common.Parameters.Options
{
    public interface IOptionsParameters
    {
        IEnumerable<string> Properties { get; set; }
    }
}