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

        private readonly string _sort = null;

        public IList<ISortingPair> Sorters
        { 
            get; 
            private set; 
        }

        public SortingParameters(string sort)
        {
            _sort = sort;
        }

        public void Initialize()
        {
            Sorters = new List<ISortingPair>();

            if (string.IsNullOrWhiteSpace(_sort)) return;
            string[] sortingRequests = _sort.Split(',');

            foreach (string sortingRequest in sortingRequests)
            {
                IList<string> sortParams = sortingRequest.Split(SortingParametersSeparator).ToList();
                if (sortParams.Count < 1)
                    throw new ArgumentNullException("Sorting field or direction.");

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