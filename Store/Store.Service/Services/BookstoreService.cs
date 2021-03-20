using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Models;
using Store.Model.Common.Models;
using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
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

        public Task<IBookstore> FindBookstoreByIdAsync(Guid bookstoreId, IOptionsParameters options)
        {
            return _unitOfWork.BookstoreRepository.FindByIdWithProjectionAsync<BookstoreDto>(bookstoreId, options);
        }

        public Task<IPagedList<IBook>> FindBooksByBookstoreIdAsync(Guid bookstoreId, IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            return _unitOfWork.BookRepository.FindByBookIdAsync(bookstoreId, filter, paging, sorting, options);
        }

        public Task<IEnumerable<IBookstore>> GetBookstoresAsync(IOptionsParameters options)
        {
            return _unitOfWork.BookstoreRepository.GetWithProjectionAsync<BookstoreDto>(options);
        }

        public Task<IPagedList<IBookstore>> FindBookstoresAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            return _unitOfWork.BookstoreRepository.FindBookstoresAsync<BookstoreDto>(filter, paging, sorting, options);
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