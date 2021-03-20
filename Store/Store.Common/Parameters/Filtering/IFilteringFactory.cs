namespace Store.Common.Parameters.Filtering
{
    public interface IFilteringFactory
    {
        T Create<T>() where T : IFilteringParameters;

        T Create<T>(string searchString) where T : IFilteringParameters;
    }
}