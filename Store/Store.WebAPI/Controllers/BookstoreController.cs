using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;

using Store.Models;
using Store.Model.Common.Models;
using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Common.Parameters;
using Store.Common.Parameters.Filtering;
using Store.Cache.Common;
using Store.WebAPI.Models;
using Store.WebAPI.Models.Book;
using Store.WebAPI.Models.Bookstore;
using Store.WebAPI.Constants;
using Store.WebAPI.Infrastructure.Authorization.Attributes;
using Store.Service.Common.Services;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/bookstores")]
    public class BookstoreController : ApplicationControllerBase
    {
        private readonly IBookstoreService _bookstoreService;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        private readonly ICacheProvider _cacheProvider;

        public BookstoreController
        (
            IBookstoreService bookstoreService,
            IBookService bookService,
            IMapper mapper, 
            ICacheManager cacheManager,
            IQueryUtilityFacade queryUtilityFacade
        ) : base(queryUtilityFacade)
        {
            _bookstoreService = bookstoreService;
            _bookService = bookService;
            _mapper = mapper;
            _cacheProvider = cacheManager.CacheProvider;
        }

        /// <summary>Retrieves the bookstore.</summary>
        /// <param name="bookstoreId">The bookstore identifier.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("{bookstoreId:guid}")]
        [Produces("application/json")]
        [SectionAuthorization(SectionType.Bookstore, AccessType.Read)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid bookstoreId, [FromQuery] string includeProperties = DefaultParameters.IncludeProperties)
        {        
            if (bookstoreId == Guid.Empty)
                return BadRequest();

            BookstoreExtendedDTO bookstore = await _bookstoreService.FindBookstoreByKeyAsync
            (
                bookstoreId,
                options: OptionsFactory.Create(ModelMapperHelper.GetPropertyMappings<BookstoreGetApiModel, BookstoreExtendedDTO>(_mapper, includeProperties))
            );

            if (bookstore != null)
                return Ok(_mapper.Map<BookstoreGetApiModel>(bookstore));

            return NotFound();
        }

        /// <summary>Retrieves books by specified search criteria.</summary>
        /// <param name="bookstoreId">The bookstore identifier.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("{bookstoreId:guid}/books")]
        [Produces("application/json")]
        [SectionAuthorization(SectionType.Bookstore, AccessType.Read)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid bookstoreId,
                                                  [FromQuery] string includeProperties = DefaultParameters.IncludeProperties,
                                                  [FromQuery] string searchString = DefaultParameters.SearchString,
                                                  [FromQuery] int pageNumber = DefaultParameters.PageNumber,
                                                  [FromQuery] int pageSize = DefaultParameters.PageSize,
                                                  [FromQuery] string sortOrder = DefaultParameters.SortOrder)
        {
            if (bookstoreId == Guid.Empty)
                return BadRequest();

            IPagedList<IBook> bookstores = await _bookService.FindBooksByBookstoreIdAsync
            (
                bookstoreId,
                filter: FilteringFactory.Create<IFilteringParameters>(searchString),
                paging: PagingFactory.Create(pageNumber, pageSize),
                sorting: SortingFactory.Create(ModelMapperHelper.GetSortPropertyMappings<BookGetApiModel, IBook>(_mapper, sortOrder)),
                options: OptionsFactory.Create(ModelMapperHelper.GetPropertyMappings<BookGetApiModel, IBook>(_mapper, includeProperties))
            );

            if (bookstores != null)
            {
                return Ok(_mapper.Map<PagedApiResponse<BookGetApiModel>>(bookstores));
            }

            return NoContent();
        }

        /// <summary>Retrieves all bookstore from cache or the database.</summary>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("all")]
        [Produces("application/json")]
        [SectionAuthorization(SectionType.Bookstore, AccessType.Read)]
        public async Task<IActionResult> GetAsync([FromQuery] string includeProperties = DefaultParameters.IncludeProperties)
        {
            Task<IEnumerable<BookstoreExtendedDTO>> GetBookstoresFuncAsync()
            {
                return _bookstoreService.GetBookstoresAsync
                (
                    sorting: SortingFactory.Create(ModelMapperHelper.GetSortPropertyMappings<BookstoreGetApiModel, BookstoreExtendedDTO>(_mapper, DefaultParameters.SortOrder)),
                    options: OptionsFactory.Create(ModelMapperHelper.GetPropertyMappings<BookstoreGetApiModel, BookstoreExtendedDTO>(_mapper, includeProperties))
                );
            }

            IEnumerable<BookstoreExtendedDTO> bookstores;

            // Need to fetch from the database as we're not storing bookstore prefetch data in cache
            if (includeProperties?.Length == 0)
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

        /// <summary>Retrieves bookstores by specified search criteria.</summary>
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
        [SectionAuthorization(SectionType.Bookstore, AccessType.Read)]
        public async Task<IActionResult> GetAsync([FromQuery] string includeProperties = DefaultParameters.IncludeProperties,
                                                  [FromQuery] string searchString = DefaultParameters.SearchString,
                                                  [FromQuery] int pageNumber = DefaultParameters.PageNumber,
                                                  [FromQuery] int pageSize = DefaultParameters.PageSize,
                                                  [FromQuery] string sortOrder = DefaultParameters.SortOrder)
        {
            IPagedList<BookstoreExtendedDTO> bookstores = await _bookstoreService.FindBookstoresAsync
            (
                filter: FilteringFactory.Create<IFilteringParameters>(searchString),
                paging: PagingFactory.Create(pageNumber, pageSize),
                sorting: SortingFactory.Create(ModelMapperHelper.GetSortPropertyMappings<BookstoreGetApiModel, BookstoreExtendedDTO>(_mapper, sortOrder)),
                options: OptionsFactory.Create(ModelMapperHelper.GetPropertyMappings<BookstoreGetApiModel, BookstoreExtendedDTO>(_mapper, includeProperties))
            );

            if (bookstores != null)
            {
                return Ok(_mapper.Map<PagedApiResponse<BookstoreGetApiModel>>(bookstores));
            }

            return NoContent();
        }


        /// <summary>Creates a new bookstore.</summary>
        /// <param name="bookstoreModel">The bookstore model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Consumes("application/json")]
        [SectionAuthorization(SectionType.Bookstore, AccessType.Create)]
        public async Task<IActionResult> PostAsync([FromBody] BookstorePostApiModel bookstoreModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            IBookstore bookstore = _mapper.Map<IBookstore>(bookstoreModel);
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

        /// <summary>Updates the bookstore.</summary>
        /// <param name="bookstoreId">The bookstore identifier.</param>
        /// <param name="bookstoreModel">The bookstore model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Route("{bookstoreId:guid}")]
        [Consumes("application/json")]
        [SectionAuthorization(SectionType.Bookstore, AccessType.Update)]
        public async Task<IActionResult> PatchAsync([FromRoute] Guid bookstoreId, [FromBody] BookstorePatchApiModel bookstoreModel)
        {
            if (bookstoreId == Guid.Empty || !ModelState.IsValid)
                return BadRequest();

            IBookstore bookstore = _mapper.Map<IBookstore>(bookstoreModel);
            bookstore.Id = bookstoreId;

            ResponseStatus result = await _bookstoreService.UpdateBookstoreAsync(bookstore);

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

        /// <summary>Deletes the bookstore.</summary>
        /// <param name="bookstoreId">The bookstore identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpDelete]
        [Route("{bookstoreId:guid}")]
        [SectionAuthorization(SectionType.Bookstore, AccessType.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid bookstoreId)
        {
            if (bookstoreId == Guid.Empty)
                return BadRequest();

            ResponseStatus result = await _bookstoreService.DeleteBookstoreAsync(bookstoreId);

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