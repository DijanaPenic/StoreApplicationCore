using System.Collections.Generic;

namespace Store.Common.Parameters.Sorting
{
    public interface ISortingParameters
    {
        IList<ISortingPair> Sorters { get; }

        void Initialize();
    }
}