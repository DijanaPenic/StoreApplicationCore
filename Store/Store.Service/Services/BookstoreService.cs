using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using X.PagedList;

using Store.Models;
using Store.Model.Common.Models;
using Store.Common.Enums;
using Store.Repository.Common.Core;
using Store.Service.Common.Services;

namespace Store.Services
{
    public class BookstoreService : IBookstoreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookstoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IBookstore> FindBookstoreByIdAsync(Guid bookstoreId, params string[] includeProperties)
        {
            return _unitOfWork.BookstoreRepository.FindByIdWithProjectionAsync<BookstoreDto>(bookstoreId, includeProperties);
        }

        public Task<IPagedList<IBook>> FindBooksByBookstoreIdAsync(Guid bookstoreId, string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize)
        {
            return _unitOfWork.BookRepository.FindByBookstoreIdAsync(bookstoreId, searchString, isDescendingSortOrder, sortOrderProperty, pageNumber, pageSize);
        }

        public Task<IEnumerable<IBookstore>> GetBookstoresAsync(params string[] includeProperties)
        {
            return _unitOfWork.BookstoreRepository.GetWithProjectionAsync<BookstoreDto>(includeProperties);
        }

        public Task<IPagedList<IBookstore>> FindBookstoresAsync(string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize, params string[] includeProperties)
        {
            Expression<Func<IBookstore, bool>> filterExpression = string.IsNullOrEmpty(searchString) ? (Expression<Func<IBookstore, bool>>)null : bs => bs.Name.Contains(searchString) || bs.Location.Contains(searchString);

            return _unitOfWork.BookstoreRepository.FindWithProjectionAsync<BookstoreDto>(filterExpression, sortOrderProperty, isDescendingSortOrder, pageNumber, pageSize, includeProperties);
        }

        public async Task<ResponseStatus> UpdateBookstoreAsync(IBookstore bookstore)
        {
            ResponseStatus status = await _unitOfWork.BookstoreRepository.UpdateAsync(bookstore);

            return await _unitOfWork.SaveChangesAsync(status);
        }

        public async Task<ResponseStatus> UpdateBookstoreAsync(Guid bookstoreId, IBookstore bookstore)
        {
            ResponseStatus status = await _unitOfWork.BookstoreRepository.UpdateAsync(bookstoreId, bookstore);

            return await _unitOfWork.SaveChangesAsync(status);
        }

        public async Task<ResponseStatus> InsertBookstoreAsync(IBookstore bookstore)
        {
            ResponseStatus status = await _unitOfWork.BookstoreRepository.AddAsync(bookstore);

            return await _unitOfWork.SaveChangesAsync(status);
        }

        public async Task<ResponseStatus> DeleteBookstoreAsync(Guid bookstoreId)
        {
            ResponseStatus status = await _unitOfWork.BookstoreRepository.DeleteByIdAsync(bookstoreId);

            return await _unitOfWork.SaveChangesAsync(status);
        }
    }
}