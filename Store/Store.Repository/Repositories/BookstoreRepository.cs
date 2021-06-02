using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.Entities;
using Store.DAL.Context;
using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Repository.Core;
using Store.Repository.Common.Repositories;
using Store.Model.Common.Models;

namespace Store.Repositories
{
    internal class BookstoreRepository : GenericRepository, IBookstoreRepository
    {
        private DbSet<BookstoreEntity> _dbSet => DbContext.Set<BookstoreEntity>();

        public BookstoreRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public Task<IPagedList<IBookstore>> FindBookstoresAsync<TDestination>(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            Expression<Func<IBookstore, bool>> filterExpression = string.IsNullOrEmpty(filter.SearchString)
                                                                  ? (Expression<Func<IBookstore, bool>>)null
                                                                  : bs => bs.Name.Contains(filter.SearchString) || bs.Location.Contains(filter.SearchString);

            return FindWithProjectionAsync<IBookstore, BookstoreEntity, TDestination>(filterExpression, paging, sorting, options);
        }

        public Task<ResponseStatus> UpdateBookstoreAsync(Guid id, IBookstore model)
        {
            return UpdateAsync<IBookstore, BookstoreEntity>(id, model);
        }

        public Task<ResponseStatus> AddBookstoreAsync(IBookstore model)
        {
            return AddAsync<IBookstore, BookstoreEntity>(model);
        }

        public Task<ResponseStatus> DeleteBookstoreByIdAsync(Guid id)
        {
            return DeleteByIdAsync<IBookstore, BookstoreEntity>(id);
        }
    }
}