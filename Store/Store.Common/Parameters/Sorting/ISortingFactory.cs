namespace Store.Common.Parameters.Sorting
{
    public interface ISortingFactory
    {
        ISortingParameters Create(string sort);
    }
}