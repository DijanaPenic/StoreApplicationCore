using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using LinqKit;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.Entities;
using Store.DAL.Context;
using Store.Common.Enums;
using Store.Model.Common.Models;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Repository.Core;
using Store.Repository.Models;
using Store.Repository.Common.Repositories;

namespace Store.Repositories
{
    internal class BookstoreRepository : GenericRepository, IBookstoreRepository
    {
        private DbSet<BookstoreEntity> _dbSet => DbContext.Set<BookstoreEntity>();

        public BookstoreRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public Task<IPagedList<IBookstore>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            ExpressionStarter<IBookstore> predicate = PredicateBuilder.New<IBookstore>();

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                predicate.And(b => b.Name.Contains(filter.SearchString) || b.Location.Contains(filter.SearchString));
            }

            return FindWithProjectionAsync<IBookstore, BookstoreEntity, BookstoreDTO>(predicate, paging, sorting, options);
        }

        public Task<IEnumerable<IBookstore>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetUsingProjectionAsync<IBookstore, BookstoreEntity, BookstoreDTO>(sorting, options);
        }

        public Task<IBookstore> FindByKeyAsync(Guid key, IOptionsParameters options = null)
        {
            return FindByKeyUsingProjectionAsync<IBookstore, BookstoreEntity, BookstoreDTO>(options, key);
        }

        public Task<ResponseStatus> UpdateAsync(IBookstore model)
        {
            return UpdateAsync<IBookstore, BookstoreEntity>(model, model.Id);
        }

        public Task<ResponseStatus> AddAsync(IBookstore model)
        {
            return AddAsync<IBookstore, BookstoreEntity>(model);
        }

        public Task<ResponseStatus> DeleteByKeyAsync(Guid key)
        {
            return DeleteByKeyAsync<IBookstore, BookstoreEntity>(key);
        }
    }
}