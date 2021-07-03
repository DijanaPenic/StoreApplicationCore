using System;
using System.Linq;
using System.Collections.Generic;

namespace Store.Common.Parameters.Sorting
{
    public class SortingParameters : ISortingParameters
    {
        public const string AscendingDirection = "asc";
        public const string DescendingDirection = "desc";
        public const char SortingParametersSeparator = '|';

        // Setter is not private because of the model mapper (the ModelMapperHelper class)
        public IList<ISortingPair> Sorters { get; set; }

        public SortingParameters(IReadOnlyCollection<string> sort)
        {
            InitializeSorting(sort);
        }

        private void InitializeSorting(IReadOnlyCollection<string> sort)
        {
            if (sort.Count == 0) return;

            Sorters = new List<ISortingPair>();   

            foreach (string sortingRequest in sort)
            {
                IList<string> sortParams = sortingRequest.Split(SortingParametersSeparator).ToList();
                if (sortParams.Count < 1)
                    throw new ArgumentNullException($"Not a valid sorting format.");

                if (string.IsNullOrWhiteSpace(sortParams[0])) continue;
                
                SortingPair sortingPair = new(
                    orderBy: sortParams[0],
                    ascending: sortParams.Count == 1 || (sortParams[1].ToLowerInvariant().StartsWith(AscendingDirection))
                );

                Sorters.Add(sortingPair);
            }
        }
    }
}