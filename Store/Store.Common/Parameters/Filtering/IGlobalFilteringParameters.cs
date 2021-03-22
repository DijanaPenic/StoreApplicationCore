using System.Collections.Generic;

using Store.Common.Enums;

namespace Store.Common.Parameters.Filtering
{
    public interface IGlobalFilteringParameters : IFilteringParameters
    {
        IList<SectionType> SearchTypes { get; set; }
    }
}