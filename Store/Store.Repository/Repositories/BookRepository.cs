using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.Entities;
using Store.DAL.Context;
using Store.Common.Enums;
using Store.Common.Extensions;
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

        public Task<IPagedList<IBook>> FindBooksAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            Expression<Func<IBook, bool>> filterExpression = string.IsNullOrEmpty(filter.SearchString)
                                                ? (Expression<Func<IBook, bool>>)null
                                                : b => b.Name.Contains(filter.SearchString) || b.Author.Contains(filter.SearchString) || b.Bookstore.Name.Contains(filter.SearchString);

            return FindAsync<IBook, BookEntity>(filterExpression, paging, sorting, options);
        }

        public Task<IPagedList<IBook>> FindBooksByBookstoreIdAsync(Guid bookstoreId, IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            Expression<Func<IBook, bool>> filterExpression = b => b.BookstoreId == bookstoreId;

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                filterExpression = filterExpression.And(b => b.Name.Contains(filter.SearchString) || b.Author.Contains(filter.SearchString));
            }

            return FindAsync<IBook, BookEntity>(filterExpression, paging, sorting, options);
        }

        public Task<IEnumerable<IBook>> GetBooksAsync(IOptionsParameters options)
        {
            return GetAsync<IBook, BookEntity>(options);
        }

        public Task<IBook> FindBookByIdAsync(Guid id, IOptionsParameters options) 
        {
            return FindByIdAsync<IBook, BookEntity>(id, options);
        }

        public Task<ResponseStatus> UpdateBookAsync(Guid id, IBook model)
        {
            return UpdateAsync<IBook, BookEntity>(id, model);
        }

        public Task<ResponseStatus> AddBookAsync(IBook model)
        {
            return AddAsync<IBook, BookEntity>(model);
        }

        public Task<ResponseStatus> DeleteBookByIdAsync(Guid id)
        {
            return DeleteByIdAsync<IBook, BookEntity>(id);
        }
    }
}