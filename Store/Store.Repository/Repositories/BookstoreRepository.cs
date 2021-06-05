using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.Models;
using Store.Model.Common.Models;
using Store.Entities;
using Store.DAL.Context;
using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Repository.Core;
using Store.Repository.Common.Repositories;

namespace Store.Repositories
{
    internal class BookstoreRepository : GenericRepository, IBookstoreRepository
    {
        private DbSet<BookstoreEntity> _dbSet => DbContext.Set<BookstoreEntity>();

        public BookstoreRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public Task<IPagedList<IBookstore>> FindBookstoresAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            return FindWithProjectionAsync<IBookstore, BookstoreEntity, BookstoreDTO>(GetFilterExpression(filter), paging, sorting, options);
        }

        public Task<IEnumerable<IBookstore>> FindBookstoresAsync(IFilteringParameters filter, ISortingParameters sorting, IOptionsParameters options)
        {
            return FindWithProjectionAsync<IBookstore, BookstoreEntity, BookstoreDTO>(GetFilterExpression(filter), sorting, options);
        }

        public Task<IEnumerable<IBookstore>> GetBookstoresAsync(IOptionsParameters options)
        {
            return GetWithProjectionAsync<IBookstore, BookstoreEntity, BookstoreDTO>(options);
        }

        public Task<IBookstore> FindBookstoreByIdAsync(Guid id, IOptionsParameters options)
        {
            return FindByIdWithProjectionAsync<IBookstore, BookstoreEntity, BookstoreDTO>(id, options);
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

        private static Expression<Func<IBookstore, bool>> GetFilterExpression(IFilteringParameters filter)
        {
            return string.IsNullOrEmpty(filter.SearchString)
                ? (Expression<Func<IBookstore, bool>>)null
                : bs => bs.Name.Contains(filter.SearchString) || bs.Location.Contains(filter.SearchString);
        }
    }
}