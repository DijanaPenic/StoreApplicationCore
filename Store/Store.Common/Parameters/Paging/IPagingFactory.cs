namespace Store.Common.Parameters.Paging
{
    public interface IPagingFactory
    {
        IPagingParameters Create(int pageNumber, int pageSize);
    }
}