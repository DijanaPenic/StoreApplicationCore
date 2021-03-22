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

        private readonly string[] _sort = null;

        // Setter is not private because of the model mapper (the ModelMapperHelper class)
        public IList<ISortingPair> Sorters { get; set; }

        public SortingParameters(string[] sort)
        {
            _sort = sort;
            Initialize();
        }

        private void Initialize()
        {
            if (_sort.Length == 0) return;

            Sorters = new List<ISortingPair>();   

            foreach (string sortingRequest in _sort)
            {
                IList<string> sortParams = sortingRequest.Split(SortingParametersSeparator).ToList();
                if (sortParams.Count < 1)
                    throw new ArgumentNullException("Not a valid sorting format.");

                if (!string.IsNullOrWhiteSpace(sortParams[0]))
                {
                    SortingPair sortingPair = new SortingPair
                    (
                        orderBy: sortParams[0],
                        ascending: sortParams.Count < 2 || (sortParams[1].ToLowerInvariant().StartsWith(AscendingDirection))
                    );

                    Sorters.Add(sortingPair);
                }
            }
        }
    }
}