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
using Microsoft.AspNetCore.Http;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
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
                return Ok(_mapper.Map<PaginationEntity<BookGetApiModel>>(books));
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]BookPostApiModel bookModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            IBook book = _mapper.Map<IBook>(bookModel);
            ResponseStatus result = await _bookService.AddBookAsync(book);

            switch (result)
            {
                case ResponseStatus.Success:
                    return StatusCode(StatusCodes.Status201Created);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch]
        [Route("{id:guid}")]
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
                    return StatusCode(StatusCodes.Status500InternalServerError);
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
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}