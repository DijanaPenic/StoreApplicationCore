using System;

namespace Store.Common.Parameters.Paging
{
    public class PagingFactory : IPagingFactory
    {
        public IPagingParameters Create(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Parameter must be greater than 1.", nameof(pageNumber));

            if (pageSize < 1)
                throw new ArgumentException("Parameter must be greater than 1.", nameof(pageSize));

            return new PagingParameters(pageNumber, pageSize);
        }
    }
}