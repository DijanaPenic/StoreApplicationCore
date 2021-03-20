namespace Store.Common.Parameters.Sorting
{
    public interface ISortingPair
    {
        bool Ascending { get; }

        string OrderBy { get; set; }

        string GetSortExpression();
    }
}