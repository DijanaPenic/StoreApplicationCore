namespace Store.Common.Parameters.Paging
{
    public class PagingParameters : IPagingParameters
    {
        public int PageNumber { get; }

        public int PageSize { get; }

        public PagingParameters(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}