using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Store.Common.Enums;
using Store.Common.Helpers.Identity;
using Store.Model.Common.Models;
using Store.Service.Common.Services;
using Store.WebAPI.Models.GlobalSearch;
using Store.WebAPI.Infrastructure.Authorization.Attributes;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/global-search")]
    [UserAuthorization(RoleHelper.Admin, RoleHelper.Customer, RoleHelper.StoreManager)]
    public class GlobalSearchController : ControllerBase
    {
        private readonly IGlobalSearchService _globalSearchService;
        private readonly IMapper _mapper;

        public GlobalSearchController(IGlobalSearchService globalSearchService, IMapper mapper)
        {
            _globalSearchService = globalSearchService;
            _mapper = mapper;
        }

        /// <summary>Performs a global search.</summary>
        /// <param name="searchString">The search string.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return BadRequest();

            // Configure search types per roles (Note: all roles can search books)
            IList<ModuleType> searchTypes = new List<ModuleType> { ModuleType.Book, ModuleType.Bookstore };

            // Admin and Store Manager roles can search bookstores 
            if (User.IsInRole(RoleHelper.Admin) || User.IsInRole(RoleHelper.StoreManager))
            {
                searchTypes.Add(ModuleType.Bookstore);
            }

            IEnumerable<ISearchItem> searchResults = await _globalSearchService.FindAsync(searchString, searchTypes);

            if (searchResults != null)
                return Ok(_mapper.Map<IEnumerable<SearchItemGetApiModel>>(searchResults));

            return NoContent();
        }
    }
}