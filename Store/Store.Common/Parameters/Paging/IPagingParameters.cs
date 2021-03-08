namespace Store.Common.Parameters.Paging
{
    public interface IPagingParameters
    {
        int PageNumber { get; }

        int PageSize { get; }
    }
}