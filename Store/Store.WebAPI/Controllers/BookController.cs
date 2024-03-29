﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;

using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Common.Parameters;
using Store.Common.Parameters.Filtering;
using Store.WebAPI.Constants;
using Store.WebAPI.Models;
using Store.WebAPI.Models.Book;
using Store.WebAPI.Infrastructure.Authorization.Attributes;
using Store.Model.Common.Models;
using Store.Service.Common.Services;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : ApplicationControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BookController
        (
            IBookService bookService, 
            IMapper mapper,
            IQueryUtilityFacade queryUtilityFacade
        ) : base(queryUtilityFacade)
        {
            _bookService = bookService;
            _mapper = mapper;
        }
        
        /// <summary>Creates a new book.</summary>
        /// <param name="bookModel">The book model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Consumes("application/json")]
        [SectionAuthorization(SectionType.Book, AccessType.Create)]
        public async Task<IActionResult> PostAsync([FromBody] BookPostApiModel bookModel)
        {
            IBook book = _mapper.Map<IBook>(bookModel);
            ResponseStatus result = await _bookService.AddBookAsync(book);

            return result switch
            {
                ResponseStatus.Success => Created(),
                _ => InternalServerError()
            };
        }
        
        /// <summary>Retrieves books by specified search criteria.</summary>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Produces("application/json")]
        [SectionAuthorization(SectionType.Book, AccessType.Read)]
        public async Task<IActionResult> GetAsync([FromQuery] string includeProperties = DefaultParameters.IncludeProperties, 
            [FromQuery] string searchString = DefaultParameters.SearchString,
            [FromQuery] int pageNumber = DefaultParameters.PageNumber,
            [FromQuery] int pageSize = DefaultParameters.PageSize,
            [FromQuery] string sortOrder = DefaultParameters.SortOrder)
        {
            IPagedList<IBook> books = await _bookService.FindBooksAsync
            (
                filter: FilteringFactory.Create<IFilteringParameters>(searchString), 
                paging: PagingFactory.Create(pageNumber, pageSize), 
                sorting: SortingFactory.Create(ModelMapperHelper.GetSortPropertyMappings<BookGetApiModel, IBook>(_mapper, sortOrder)), 
                options: OptionsFactory.Create(ModelMapperHelper.GetPropertyMappings<BookGetApiModel, IBook>(_mapper, includeProperties))
            );

            if (books != null)
            {
                return Ok(_mapper.Map<PagedApiResponse<BookGetApiModel>>(books));
            }

            return NoContent();
        }

        /// <summary>Retrieves the book by identifier.</summary>
        /// <param name="bookId">The book identifier.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("{bookId:guid}")]
        [Produces("application/json")]
        [SectionAuthorization(SectionType.Book, AccessType.Read)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid bookId, [FromQuery] string includeProperties = DefaultParameters.IncludeProperties)
        {
            IBook book = await _bookService.FindBookByKeyAsync
            (
                bookId,
                options: OptionsFactory.Create(ModelMapperHelper.GetPropertyMappings<BookGetApiModel, IBook>(_mapper, includeProperties))
            );

            if (book != null)
                return Ok(_mapper.Map<BookGetApiModel>(book)); 

            return NotFound("Book cannot be found.");
        }

        /// <summary>Updates the book.</summary>
        /// <param name="bookId">The book identifier.</param>
        /// <param name="bookModel">The book model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Route("{bookId:guid}")]
        [Consumes("application/json")]
        [SectionAuthorization(SectionType.Book, AccessType.Update)]
        public async Task<IActionResult> PatchAsync([FromRoute] Guid bookId, [FromBody] BookPatchApiModel bookModel)
        {
            IBook book = await _bookService.FindBookByKeyAsync(bookId);
            if (book == null)
            {
                return NotFound("Book cannot be found.");
            }

            _mapper.Map(bookModel, book);

            ResponseStatus result = await _bookService.UpdateBookAsync(book);

            return result switch
            {
                ResponseStatus.Success => NoContent(),
                _ => InternalServerError()
            };
        }

        /// <summary>Deletes the book.</summary>
        /// <param name="bookId">The book identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpDelete]
        [Route("{bookId:guid}")]
        [SectionAuthorization(SectionType.Book, AccessType.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid bookId)
        {
            ResponseStatus result = await _bookService.DeleteBookByKeyAsync(bookId);

            return result switch
            {
                ResponseStatus.NotFound => NotFound("Book cannot be found."),
                ResponseStatus.Success => NoContent(),
                _ => InternalServerError()
            };
        }
    }
}