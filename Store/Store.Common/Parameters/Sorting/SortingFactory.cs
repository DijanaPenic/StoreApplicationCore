namespace Store.Common.Parameters.Sorting
{
    public class SortingFactory : ISortingFactory
    {
        public SortingFactory()
        {
        }

        public ISortingParameters Create(string[] sort)
        {
            return new SortingParameters(sort);
        }
    }
}