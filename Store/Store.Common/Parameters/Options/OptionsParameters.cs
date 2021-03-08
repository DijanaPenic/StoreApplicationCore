using System.Collections.Generic;

namespace Store.Common.Parameters.Options
{
    public class OptionsParameters : IOptionsParameters
    {
        public IEnumerable<string> Properties { get; set; }

        public OptionsParameters(IEnumerable<string> properties = null)
        {
            Properties = properties;
        }
    }
}