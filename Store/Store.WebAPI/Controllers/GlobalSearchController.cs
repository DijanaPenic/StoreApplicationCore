using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Store.Common.Enums;
using Store.Common.Parameters;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models;
using Store.Service.Common.Services;
using Store.WebAPI.Models.GlobalSearch;
using Store.WebAPI.Infrastructure.Authorization.Extensions;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/global-search")]
    public class GlobalSearchController : ApplicationControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IGlobalSearchService _globalSearchService;
        private readonly IMapper _mapper;

        public GlobalSearchController
        (
            IAuthorizationService authorizationService,
            IGlobalSearchService globalSearchService, 
            IMapper mapper,
            IQueryUtilityFacade queryUtilityFacade
        ) : base(queryUtilityFacade)
        {
            _authorizationService = authorizationService;
            _globalSearchService = globalSearchService;
            _mapper = mapper;
        }

        /// <summary>Performs a global search.</summary>
        /// <param name="searchString">The search string.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync([FromQuery] string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return BadRequest("Must provide a search string.");
            }

            IList<SectionType> searchTypes = await GetSearchableSectionTypesAsync(new SectionType[] 
            { 
                SectionType.Book, 
                SectionType.Bookstore 
            }).ToArrayAsync();

            if(searchTypes.Count == 0)
            {
                return Forbid();
            }

            IGlobalFilteringParameters filtering = FilteringFactory.Create<IGlobalFilteringParameters>(searchString);
            filtering.SearchTypes = searchTypes;

            IEnumerable<ISearchItem> searchResults = await _globalSearchService.FindAsync(filtering);

            if (searchResults != null)
            {
                return Ok(_mapper.Map<IEnumerable<SearchItemGetApiModel>>(searchResults));
            }

            return NoContent();
        }

        private async IAsyncEnumerable<SectionType> GetSearchableSectionTypesAsync(SectionType[] sectionTypes)
        {
            foreach (SectionType sectionType in sectionTypes)
            {
                if ((await _authorizationService.AuthorizeAsync(User, sectionType, AccessType.Read)).Succeeded)
                {
                    yield return sectionType;
                }
            }
        }
    }
}