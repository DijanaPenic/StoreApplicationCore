using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;

using Store.Models.Api;
using Store.Models.Api.Book;
using Store.Models.Api.Bookstore;
using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Cache.Common;
using Store.WebAPI.Constants;
using Store.Model.Common.Models;
using Store.Service.Common.Services;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookstoreController : ExtendedControllerBase
    {
        private readonly IBookstoreService _bookstoreService;
        private readonly IMapper _mapper;
        private readonly ICacheProvider _cacheProvider;

        public BookstoreController(IBookstoreService bookstoreService, IMapper mapper, ICacheManager cacheManager)
        {
            _bookstoreService = bookstoreService;
            _mapper = mapper;
            _cacheProvider = cacheManager.CacheProvider;
        }

        [HttpGet]
        [Route("{id:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync([FromRoute]Guid id, [FromQuery]string[] includeProperties)
        {        
            if (id == Guid.Empty)
                return BadRequest();

            IBookstore bookstore = await _bookstoreService.FindBookstoreByIdAsync(id, ModelMapperHelper.GetPropertyMappings<BookstoreGetApiModel, IBookstore>(_mapper, includeProperties));

            if (bookstore != null)
                return Ok(_mapper.Map<BookstoreGetApiModel>(bookstore));

            return NotFound();
        }

        [HttpGet]
        [Route("{id:guid}/books")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync([FromRoute]Guid id, string searchString = DefaultParameters.SearchString, int pageNumber = DefaultParameters.PageNumber, int pageSize = DefaultParameters.PageSize,
                                                  bool isDescendingSortOrder = DefaultParameters.IsDescendingSortOrder, string sortOrderProperty = nameof(BookGetApiModel.Name))
        {
            if (id == Guid.Empty)
                return BadRequest();

            IPagedList<IBook> bookstores = await _bookstoreService.FindBooksByBookstoreIdAsync
            (
                id,
                searchString,
                isDescendingSortOrder,
                ModelMapperHelper.GetPropertyMapping<BookGetApiModel, IBook>(_mapper, sortOrderProperty),
                pageNumber,
                pageSize
            );

            if (bookstores != null)
            {
                return Ok(_mapper.Map<PaginationEntity<BookGetApiModel>>(bookstores));
            }

            return NoContent();
        }

        [HttpGet]
        [Route("all")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync([FromQuery]string[] includeProperties)
        {
            Task<IEnumerable<IBookstore>> GetBookstoresFuncAsync()
            {
                return _bookstoreService.GetBookstoresAsync(ModelMapperHelper.GetPropertyMappings<BookstoreGetApiModel, IBookstore>(_mapper, includeProperties));
            }

            IEnumerable<IBookstore> bookstores;

            // Need to fetch from the database as we're not storing bookstore prefetch data in cache
            if (includeProperties.Length == 0)
            {
                bookstores = await _cacheProvider.GetOrAddAsync
                (
                    CacheParameters.Keys.AllBookstores,
                    GetBookstoresFuncAsync,
                    DateTimeOffset.MaxValue,
                    CacheParameters.Groups.Bookstores
                );
            }
            else
            {
                bookstores = await GetBookstoresFuncAsync();
            }

            if (bookstores != null)
                return Ok(_mapper.Map<IEnumerable<BookstoreGetApiModel>>(bookstores));

            return NoContent();
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync([FromQuery]string[] includeProperties, string searchString = DefaultParameters.SearchString, int pageNumber = DefaultParameters.PageNumber, int pageSize = DefaultParameters.PageSize,
                                                  bool isDescendingSortOrder = DefaultParameters.IsDescendingSortOrder, string sortOrderProperty = nameof(BookstoreGetApiModel.Name))
        {
            IPagedList<IBookstore> bookstores = await _bookstoreService.FindBookstoresAsync
            (
                searchString,
                isDescendingSortOrder,
                ModelMapperHelper.GetPropertyMapping<BookstoreGetApiModel, IBookstore>(_mapper, sortOrderProperty),
                pageNumber,
                pageSize,
                ModelMapperHelper.GetPropertyMappings<BookstoreGetApiModel, IBookstore>(_mapper, includeProperties)
            );

            if (bookstores != null)
            {
                return Ok(_mapper.Map<PaginationEntity<BookstoreGetApiModel>>(bookstores));
            }

            return NoContent();
        }


        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> PostAsync([FromBody]BookstoreApiPostModel bookstoreViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            IBookstore bookstore = _mapper.Map<IBookstore>(bookstoreViewModel);
            ResponseStatus result = await _bookstoreService.InsertBookstoreAsync(bookstore);

            switch (result)
            {
                case ResponseStatus.Success:
                    _cacheProvider.Remove(CacheParameters.Keys.AllBookstores, CacheParameters.Groups.Bookstores);
                    return Created();
                default:
                    return InternalServerError();
            }
        }

        [HttpPatch]
        [Route("{id:guid}")]
        [Consumes("application/json")]
        public async Task<IActionResult> PatchAsync([FromRoute]Guid id, [FromBody]BookstorePatchApiModel bookstoreViewModel)
        {
            if (id == Guid.Empty || !ModelState.IsValid)
                return BadRequest();

            ResponseStatus result = await _bookstoreService.UpdateBookstoreAsync(id, _mapper.Map<IBookstore>(bookstoreViewModel));

            switch (result)
            {
                case ResponseStatus.NotFound:
                    return NotFound();
                case ResponseStatus.Success:
                    _cacheProvider.Remove(CacheParameters.Keys.AllBookstores, CacheParameters.Groups.Bookstores);
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

            ResponseStatus result = await _bookstoreService.DeleteBookstoreAsync(id);

            switch (result)
            {
                case ResponseStatus.NotFound:
                    return NotFound();
                case ResponseStatus.Success:
                    _cacheProvider.Remove(CacheParameters.Keys.AllBookstores, CacheParameters.Groups.Bookstores);
                    return NoContent();
                default:
                    return InternalServerError();
            }
        }
    }
}