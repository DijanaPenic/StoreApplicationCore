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
            return _unitOfWork.BookstoreRepository.FindByIdWithProjectionAsync<BookstoreDTO>(bookstoreId, includeProperties);
        }

        public Task<IPagedList<IBook>> FindBooksByBookstoreIdAsync(Guid bookstoreId, string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize)
        {
            return _unitOfWork.BookRepository.FindByBookstoreIdAsync(bookstoreId, searchString, isDescendingSortOrder, sortOrderProperty, pageNumber, pageSize);
        }

        public Task<IEnumerable<IBookstore>> GetBookstoresAsync(params string[] includeProperties)
        {
            return _unitOfWork.BookstoreRepository.GetWithProjectionAsync<BookstoreDTO>(includeProperties);
        }

        public Task<IPagedList<IBookstore>> FindBookstoresAsync(string searchString, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize, params string[] includeProperties)
        {
            Expression<Func<IBookstore, bool>> filterExpression = string.IsNullOrEmpty(searchString) ? (Expression<Func<IBookstore, bool>>)null : bs => bs.Name.Contains(searchString) || bs.Location.Contains(searchString);

            return _unitOfWork.BookstoreRepository.FindWithProjectionAsync<BookstoreDTO>(filterExpression, sortOrderProperty, isDescendingSortOrder, pageNumber, pageSize, includeProperties);
        }

        public Task<ResponseStatus> UpdateBookstoreAsync(IBookstore bookstore)
        {
            return _unitOfWork.BookstoreRepository.UpdateAsync(bookstore);
        }

        public Task<ResponseStatus> UpdateBookstoreAsync(Guid bookstoreId, IBookstore bookstore)
        {
            return _unitOfWork.BookstoreRepository.UpdateAsync(bookstoreId, bookstore);
        }

        public Task<ResponseStatus> InsertBookstoreAsync(IBookstore bookstore)
        {
            return _unitOfWork.BookstoreRepository.AddAsync(bookstore);
        }

        public Task<ResponseStatus> DeleteBookstoreAsync(Guid bookstoreId)
        {
            return _unitOfWork.BookstoreRepository.DeleteByIdAsync(bookstoreId);
        }
    }
}