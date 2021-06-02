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

        public Task<IEnumerable<IBook>> GetBooksAsync(IOptionsParameters options)
        {
            return _unitOfWork.BookRepository.GetBooksAsync(options);
        }

        public Task<IBook> FindBookByIdAsync(Guid bookId, IOptionsParameters options)
        {
            return _unitOfWork.BookRepository.FindBookByIdAsync(bookId, options);
        }

        public Task<IPagedList<IBook>> FindBooksAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            return _unitOfWork.BookRepository.FindBooksAsync(filter, paging, sorting, options);
        }

        public async Task<ResponseStatus> UpdateBookAsync(Guid bookId, IBook book)
        {
            ResponseStatus status = await _unitOfWork.BookRepository.UpdateBookAsync(bookId, book);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<ResponseStatus> AddBookAsync(IBook book)
        {
            ResponseStatus status = await _unitOfWork.BookRepository.AddBookAsync(book);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<ResponseStatus> DeleteBookAsync(Guid bookId)
        {
            ResponseStatus status = await _unitOfWork.BookRepository.DeleteBookByIdAsync(bookId);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }
    }
}