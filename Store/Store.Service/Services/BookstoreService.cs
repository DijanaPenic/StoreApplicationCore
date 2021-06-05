using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Common.Enums;
using Store.Model.Common.Models;
using Store.Repository.Common.Core;
using Store.Service.Common.Services;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Services
{
    internal class BookstoreService : IBookstoreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookstoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IBookstore> FindBookstoreByIdAsync(Guid bookstoreId, IOptionsParameters options)
        {
            return _unitOfWork.BookstoreRepository.FindBookstoreByIdAsync(bookstoreId, options);
        }

        public Task<IPagedList<IBook>> FindBooksByBookstoreIdAsync(Guid bookstoreId, IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            return _unitOfWork.BookRepository.FindBooksByBookstoreIdAsync(bookstoreId, filter, paging, sorting, options);
        }

        public Task<IEnumerable<IBookstore>> GetBookstoresAsync(IOptionsParameters options)
        {
            return _unitOfWork.BookstoreRepository.GetBookstoresAsync(options);
        }

        public Task<IPagedList<IBookstore>> FindBookstoresAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            return _unitOfWork.BookstoreRepository.FindBookstoresAsync(filter, paging, sorting, options);
        }

        public async Task<ResponseStatus> UpdateBookstoreAsync(Guid bookstoreId, IBookstore bookstore)
        {
            ResponseStatus status = await _unitOfWork.BookstoreRepository.UpdateBookstoreAsync(bookstoreId, bookstore);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<ResponseStatus> InsertBookstoreAsync(IBookstore bookstore)
        {
            ResponseStatus status = await _unitOfWork.BookstoreRepository.AddBookstoreAsync(bookstore);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<ResponseStatus> DeleteBookstoreAsync(Guid bookstoreId)
        {
            ResponseStatus status = await _unitOfWork.BookstoreRepository.DeleteBookstoreByIdAsync(bookstoreId);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }
    }
}