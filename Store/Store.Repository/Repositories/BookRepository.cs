using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LinqKit;
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
    internal class BookRepository : GenericRepository, IBookRepository
    {
        private DbSet<BookEntity> _dbSet => DbContext.Set<BookEntity>();

        public BookRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public Task<IPagedList<IBook>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            ExpressionStarter<IBook> predicate = PredicateBuilder.New<IBook>(true);

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                predicate.And(b => b.Name.Contains(filter.SearchString) || 
                                   b.Author.Contains(filter.SearchString) || 
                                   b.Bookstore.Name.Contains(filter.SearchString));
            }

            return FindAsync<IBook, BookEntity>(predicate, paging, sorting, options);
        }

        public Task<IPagedList<IBook>> FindByBookstoreIdAsync(Guid bookstoreId, IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            ExpressionStarter<IBook> predicate = PredicateBuilder.New<IBook>(b => b.BookstoreId == bookstoreId);

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                predicate.And(b => b.Name.Contains(filter.SearchString) || b.Author.Contains(filter.SearchString));
            }

            return FindAsync<IBook, BookEntity>(predicate, paging, sorting, options);
        }

        public Task<IEnumerable<IBook>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetAsync<IBook, BookEntity>(sorting, options);
        }

        public Task<IBook> FindByKeyAsync(Guid key, IOptionsParameters options = null) 
        {
            return FindByKeyAsync<IBook, BookEntity>(options, key);
        }

        public Task<ResponseStatus> UpdateAsync(IBook model)
        {
            return UpdateAsync<IBook, BookEntity>(model, model.Id);
        }

        public Task<ResponseStatus> AddAsync(IBook model)
        {
            return AddAsync<IBook, BookEntity>(model);
        }

        public Task<ResponseStatus> DeleteByKeyAsync(Guid key)
        {
            return DeleteByKeyAsync<BookEntity>(key);
        }
    }
}