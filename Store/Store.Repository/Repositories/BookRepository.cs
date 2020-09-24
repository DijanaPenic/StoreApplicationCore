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

namespace Store.Repositories
{
    public class BookRepository : GenericRepository<BookEntity, IBook>, IBookRepository
    {
        public BookRepository(StoreDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<IPagedList<IBook>> FindAsync(string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize, params string[] includeProperties)
        {
            Expression<Func<IBook, bool>> filterExpression = string.IsNullOrEmpty(searchString) ? (Expression<Func<IBook, bool>>)null : b => b.Name.Contains(searchString) || b.Author.Contains(searchString) || b.Bookstore.Name.Contains(searchString);

            return FindAsync(filterExpression, sortOrderProperty, isDescendingSortOrder, pageNumber, pageSize, includeProperties);
        }
    }
}