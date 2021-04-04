using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using AutoMapper;
using X.PagedList;

using Store.Entities;
using Store.DAL.Context;
using Store.Repository.Core;
using Store.Repository.Common.Repositories;
using Store.Model.Common.Models;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Repositories
{
    internal class BookstoreRepository : GenericRepository<BookstoreEntity, IBookstore>, IBookstoreRepository
    {
        public BookstoreRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public Task<IPagedList<IBookstore>> FindBookstoresAsync<TDestination>(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            Expression<Func<IBookstore, bool>> filterExpression = string.IsNullOrEmpty(filter.SearchString)
                                                                  ? (Expression<Func<IBookstore, bool>>)null
                                                                  : bs => bs.Name.Contains(filter.SearchString) || bs.Location.Contains(filter.SearchString);

            return FindWithProjectionAsync<TDestination>(filterExpression, paging, sorting, options);
        }
    }
}