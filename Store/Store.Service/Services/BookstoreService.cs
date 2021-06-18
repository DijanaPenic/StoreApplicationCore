using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Models;
using Store.Model.Common.Models;
using Store.Common.Enums;
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

        public Task<IBookstore> FindBookstoreByKeyAsync(Guid bookstoreId, IOptionsParameters options)
        {
            return _unitOfWork.BookstoreRepository.FindByKeyAsync(bookstoreId, options);
        }

        public Task<IPagedList<IBook>> FindBooksByBookstoreIdAsync(Guid bookstoreId, IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        public Task<BookstoreExtendedDTO> FindBookstoreByKeyAsync(Guid bookstoreId, IOptionsParameters options)
        {
            return _unitOfWork.BookstoreRepository.FindExtendedByKeyAsync(bookstoreId, options);
        }

        public Task<IEnumerable<BookstoreExtendedDTO>> GetBookstoresAsync(ISortingParameters sorting, IOptionsParameters options)
        {
            return _unitOfWork.BookstoreRepository.GetExtendedAsync(sorting, options);
        }

        public Task<IPagedList<BookstoreExtendedDTO>> FindBookstoresAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            return _unitOfWork.BookstoreRepository.FindExtendedAsync(filter, paging, sorting, options);
        }

        public async Task<ResponseStatus> UpdateBookstoreAsync(IBookstore bookstore)
        {
            ResponseStatus status = await _unitOfWork.BookstoreRepository.UpdateAsync(bookstore);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<ResponseStatus> InsertBookstoreAsync(IBookstore bookstore)
        {
            ResponseStatus status = await _unitOfWork.BookstoreRepository.AddAsync(bookstore);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<ResponseStatus> DeleteBookstoreAsync(Guid bookstoreId)
        {
            ResponseStatus status = await _unitOfWork.BookstoreRepository.DeleteByKeyAsync(bookstoreId);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }
    }
}