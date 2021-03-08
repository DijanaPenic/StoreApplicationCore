namespace Store.Common.Parameters.Paging
{
    public class PagingParameters : IPagingParameters
    {
        public int PageNumber
        {
            get;
            private set;
        }

        public int PageSize
        {
            get;
            private set;
        }

        public PagingParameters(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}