namespace Store.Common.Parameters.Sorting
{
    public class SortingFactory : ISortingFactory
    {
        public SortingFactory()
        {
        }

        public ISortingParameters Create(string sort)
        {
            ISortingParameters result = new SortingParameters(sort);
            result.Initialize();

            return result;
        }
    }
}