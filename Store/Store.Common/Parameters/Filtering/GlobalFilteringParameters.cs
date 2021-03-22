using System.Collections.Generic;

using Store.Common.Enums;

namespace Store.Common.Parameters.Filtering
{
    public class GlobalFilteringParameters : FilteringParameters, IGlobalFilteringParameters
    {
        public IList<SectionType> SearchTypes { get; set; }
    }
}