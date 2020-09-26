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
using Store.Model.Common.Models;

namespace Store.Repositories
{
    public class BookRepository : GenericRepository<BookEntity, IBook>, IBookRepository
    {
        public BookRepository(StoreDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<IPagedList<IBook>> FindByBookstoreIdAsync(Guid bookstoreId, string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize)
        {
            Expression<Func<IBook, bool>> filterExpression = b => b.BookstoreId == bookstoreId;

            if (!string.IsNullOrEmpty(searchString))
            {
                filterExpression = filterExpression.And(b => b.Name.Contains(searchString) || b.Author.Contains(searchString));
            }

            return FindAsync(filterExpression, sortOrderProperty, isDescendingSortOrder, pageNumber, pageSize);
        }
    }
}