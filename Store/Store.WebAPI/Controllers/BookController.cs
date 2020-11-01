using System;
using System.Threading.Tasks;
using AutoMapper;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;

using Store.Models.Api;
using Store.Models.Api.Book;
using Store.Model.Common.Models;
using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Common.Helpers.Identity;
using Store.WebAPI.Constants;
using Store.WebAPI.Infrastructure;
using Store.Service.Common.Services;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("book")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [AuthorizationFilter(RoleHelper.All)]
    public class BookController : ExtendedControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BookController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

        /// <summary>Retrieves the book by identifier.</summary>
        /// <param name="id">The book's identifier.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("{id:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync([FromRoute]Guid id, [FromQuery] string[] includeProperties)
        {
            if (id == Guid.Empty)
                return BadRequest();

            IBook book = await _bookService.FindBookByIdAsync(id, ModelMapperHelper.GetPropertyMappings<BookGetApiModel, IBook>(_mapper, includeProperties));

            if (book != null)
                return Ok(_mapper.Map<BookGetApiModel>(book)); 

            return NotFound();
        }

        /// <summary>Retrieves books by specified search criteria.</summary>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="isDescendingSortOrder">if set to <c>true</c> [is descending sort order].</param>
        /// <param name="sortOrderProperty">The sort order property.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync([FromQuery] string[] includeProperties, string searchString = DefaultParameters.SearchString, int pageNumber = DefaultParameters.PageNumber,
                                                  int pageSize = DefaultParameters.PageSize, bool isDescendingSortOrder = DefaultParameters.IsDescendingSortOrder, string sortOrderProperty = nameof(BookGetApiModel.Name))
        {
            IPagedList<IBook> books = await _bookService.FindBooksAsync
            (
                searchString,
                isDescendingSortOrder,
                ModelMapperHelper.GetPropertyMapping<BookGetApiModel, IBook>(_mapper, sortOrderProperty),
                pageNumber,
                pageSize,
                ModelMapperHelper.GetPropertyMappings<BookGetApiModel, IBook>(_mapper, includeProperties)
            );

            if (books != null)
            {
                return Ok(_mapper.Map<PagedApiResponse<BookGetApiModel>>(books));
            }

            return NoContent();
        }

        /// <summary>Creates a book.</summary>
        /// <param name="bookModel">The book model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> PostAsync([FromBody]BookPostApiModel bookModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            IBook book = _mapper.Map<IBook>(bookModel);
            ResponseStatus result = await _bookService.AddBookAsync(book);

            switch (result)
            {
                case ResponseStatus.Success:
                    return Created();
                default:
                    return InternalServerError();
            }
        }

        /// <summary>Updates the book.</summary>
        /// <param name="id">The identifier.</param>
        /// <param name="bookModel">The book model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Route("{id:guid}")]
        [Consumes("application/json")]
        public async Task<IActionResult> PatchAsync([FromRoute]Guid id, [FromBody]BookPatchApiModel bookModel)
        {
            if (id == Guid.Empty || !ModelState.IsValid)
                return BadRequest();

            ResponseStatus result = await _bookService.UpdateBookAsync(id, _mapper.Map<IBook>(bookModel));

            switch (result)
            {
                case ResponseStatus.NotFound:
                    return NotFound();
                case ResponseStatus.Success:
                    return NoContent();
                default:
                    return InternalServerError();
            }
        }

        /// <summary>Deletes the book.</summary>
        /// <param name="id">The book identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            ResponseStatus result = await _bookService.DeleteBookAsync(id);

            switch (result)
            {
                case ResponseStatus.NotFound:
                    return NotFound();
                case ResponseStatus.Success:
                    return NoContent();
                default:
                    return InternalServerError();
            }
        }
    }
}