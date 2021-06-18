using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models;
using Store.Repository.Common.Core;
using Store.Service.Common.Services;

namespace Store.Services
{
    internal class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<IBook>> GetBooksAsync(ISortingParameters sorting, IOptionsParameters options)
        {
            return _unitOfWork.BookRepository.GetAsync(sorting, options);
        }

        public Task<IBook> FindBookByKeyAsync(Guid bookId, IOptionsParameters options)
        {
            return _unitOfWork.BookRepository.FindByKeyAsync(bookId, options);
        }

        public Task<IPagedList<IBook>> FindBooksAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            return _unitOfWork.BookRepository.FindAsync(filter, paging, sorting, options);
        }

        public Task<IPagedList<IBook>> FindBooksByBookstoreIdAsync(Guid bookstoreId, IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            return _unitOfWork.BookRepository.FindByBookstoreIdAsync(bookstoreId, filter, paging, sorting, options);
        }

        public async Task<ResponseStatus> UpdateBookAsync(IBook book)
        {
            ResponseStatus status = await _unitOfWork.BookRepository.UpdateAsync(book);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<ResponseStatus> AddBookAsync(IBook book)
        {
            ResponseStatus status = await _unitOfWork.BookRepository.AddAsync(book);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<ResponseStatus> DeleteBookByKeyAsync(Guid bookId)
        {
            ResponseStatus status = await _unitOfWork.BookRepository.DeleteByKeyAsync(bookId);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }
    }
}