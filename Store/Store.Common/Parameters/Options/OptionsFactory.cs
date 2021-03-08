using System.Collections.Generic;

namespace Store.Common.Parameters.Options
{
    public class OptionsFactory : IOptionsFactory
    {
        public OptionsFactory()
        {
        }

        public IOptionsParameters Create(IEnumerable<string> properties = null)
        {
            return new OptionsParameters(properties);
        }
    }
}