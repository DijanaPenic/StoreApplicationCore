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
using Store.WebAPI.Constants;
using Store.Service.Common.Services;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("book")]
    public class BookController : ExtendedControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BookController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

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
                return Ok(_mapper.Map<PagedResponse<BookGetApiModel>>(books));
            }

            return NoContent();
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> PostAsync([FromBody]BookPostApiModel bookViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            IBook book = _mapper.Map<IBook>(bookViewModel);
            ResponseStatus result = await _bookService.AddBookAsync(book);

            switch (result)
            {
                case ResponseStatus.Success:
                    return Created();
                default:
                    return InternalServerError();
            }
        }

        [HttpPatch]
        [Route("{id:guid}")]
        [Consumes("application/json")]
        public async Task<IActionResult> PatchAsync([FromRoute]Guid id, [FromBody]BookPatchApiModel bookViewModel)
        {
            if (id == Guid.Empty || !ModelState.IsValid)
                return BadRequest();

            ResponseStatus result = await _bookService.UpdateBookAsync(id, _mapper.Map<IBook>(bookViewModel));

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