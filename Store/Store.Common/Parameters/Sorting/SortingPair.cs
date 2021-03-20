namespace Store.Common.Parameters.Sorting
{
    public class SortingPair : ISortingPair
    {
        public bool Ascending { get; private set; }

        public string OrderBy { get; set; }

        public SortingPair(string orderBy, bool ascending)
        {
            OrderBy = orderBy;
            Ascending = ascending;
        }

        public virtual string GetSortExpression()
        {
            return string.Format
            (
                "{0}{1}{2}", 
                OrderBy, 
                SortingParameters.SortingParametersSeparator, 
                Ascending ? SortingParameters.AscendingDirection : SortingParameters.DescendingDirection
            );
        }
    }
}