﻿using Store.Common.DataTypes;

namespace Store.Common.Parameters.Filtering
{
    public class FilteringParameters : IFilteringParameters
    {
        public DateTimeRange? DateCreated { get; set; }

        public DateTimeRange? DateUpdated { get; set; }

        public string SearchString { get; set; }

        public FilteringParameters()
        {
        }

        public FilteringParameters(string searchPhrase)
        {
            SearchString = searchPhrase;
        }
    }
}