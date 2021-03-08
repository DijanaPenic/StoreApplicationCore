using System.Collections.Generic;

namespace Store.Common.Parameters.Options
{
    public interface IOptionsFactory
    {
        IOptionsParameters Create(IEnumerable<string> properties = null);
    }
}