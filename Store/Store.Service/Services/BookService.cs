using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using X.PagedList;

using Store.Common.Enums;
using Store.Model.Common.Models;
using Store.Repository.Common.Core;
using Store.Service.Common.Services;

namespace Store.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<IBook>> GetBooksAsync(params string[] includeProperties)
        {
            return _unitOfWork.BookRepository.GetAsync(includeProperties);
        }

        public Task<IBook> FindBookByIdAsync(Guid bookId, params string[] includeProperties)
        {
            return _unitOfWork.BookRepository.FindByIdAsync(bookId, includeProperties);
        }

        public Task<IPagedList<IBook>> FindBooksAsync(string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize, params string[] includeProperties)
        {
            Expression<Func<IBook, bool>> filterExpression = string.IsNullOrEmpty(searchString) ? (Expression<Func<IBook, bool>>)null : b => b.Name.Contains(searchString) || b.Author.Contains(searchString) || b.Bookstore.Name.Contains(searchString);

            return _unitOfWork.BookRepository.FindAsync(filterExpression, sortOrderProperty, isDescendingSortOrder, pageNumber, pageSize, includeProperties);
        }

        public async Task<ResponseStatus> UpdateBookAsync(IBook book)
        {
            ResponseStatus status = await _unitOfWork.BookRepository.UpdateAsync(book);

            return await _unitOfWork.SaveChangesAsync(status);
        }

        public async Task<ResponseStatus> UpdateBookAsync(Guid bookId, IBook book)
        {
            ResponseStatus status = await _unitOfWork.BookRepository.UpdateAsync(bookId, book);

            return await _unitOfWork.SaveChangesAsync(status);
        }

        public async Task<ResponseStatus> AddBookAsync(IBook book)
        {
            ResponseStatus status = await _unitOfWork.BookRepository.AddAsync(book);

            return await _unitOfWork.SaveChangesAsync(status);
        }

        public async Task<ResponseStatus> DeleteBookAsync(Guid bookId)
        {
            ResponseStatus status = await _unitOfWork.BookRepository.DeleteByIdAsync(bookId);

            return await _unitOfWork.SaveChangesAsync(status);
        }
    }
}