using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using AutoMapper;
using X.PagedList;

using Store.Entities;
using Store.DAL.Context;
using Store.Repository.Core;
using Store.Repository.Common.Repositories;
using Store.Common.Extensions;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models;

namespace Store.Repositories
{
    public class BookRepository : GenericRepository<BookEntity, IBook>, IBookRepository
    {
        public BookRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public Task<IPagedList<IBook>> FindBooksAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            Expression<Func<IBook, bool>> filterExpression = string.IsNullOrEmpty(filter.SearchString)
                                                ? (Expression<Func<IBook, bool>>)null
                                                : b => b.Name.Contains(filter.SearchString) || b.Author.Contains(filter.SearchString) || b.Bookstore.Name.Contains(filter.SearchString);

            return FindAsync(filterExpression, paging, sorting, options);
        }

        public Task<IPagedList<IBook>> FindByBookIdAsync(Guid bookstoreId, IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            Expression<Func<IBook, bool>> filterExpression = b => b.BookstoreId == bookstoreId;

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                filterExpression = filterExpression.And(b => b.Name.Contains(filter.SearchString) || b.Author.Contains(filter.SearchString));
            }

            return FindAsync(filterExpression, paging, sorting, options);
        }
    }
}