using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Store.Service.Constants;
using Store.Common.Parameters;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Filtering;

namespace Store.WebAPI.Controllers
{
    public abstract class ApplicationControllerBase : ControllerBase
    {
        private IOptionsFactory _optionsFactory = null;
        private IPagingFactory _pagingFactory = null;
        private ISortingFactory _sortingFactory = null;
        private IFilteringFactory _filteringFactory = null;
        private readonly IQueryUtilityFacade _queryUtilityFacade;

        protected IOptionsFactory OptionsFactory => _optionsFactory ??= _queryUtilityFacade.CreateOptionsFactory();

        protected IFilteringFactory FilteringFactory => _filteringFactory ??= _queryUtilityFacade.CreateFilteringFactory();

        protected IPagingFactory PagingFactory => _pagingFactory ??= _queryUtilityFacade.CreatePagingFactory();

        protected ISortingFactory SortingFactory => _sortingFactory ??= _queryUtilityFacade.CreateSortingFactory();

        protected ApplicationControllerBase(IQueryUtilityFacade queryUtilityFacade)
        {
            _queryUtilityFacade = queryUtilityFacade;
        }

        protected IActionResult InternalServerError() => StatusCode(StatusCodes.Status500InternalServerError);

        protected IActionResult Created() => StatusCode(StatusCodes.Status201Created);

        protected bool IsCurrentUser(Guid userId) => GetUserId(User) == userId;

        protected Guid GetCurrentUserClientId() => GetClientId(User);

        protected static bool IsUser(Guid userId, ClaimsPrincipal claimsPrincipal) => GetUserId(claimsPrincipal) == userId;

        protected static Guid GetUserId(ClaimsPrincipal claimsPrincipal) => Guid.Parse(claimsPrincipal.Claims.FirstOrDefault(uc => uc.Type == ClaimTypes.NameIdentifier)?.Value);

        protected static Guid GetClientId(ClaimsPrincipal claimsPrincipal) => Guid.Parse(claimsPrincipal.Claims.FirstOrDefault(uc => uc.Type == ApplicationClaimTypes.ClientIdentifier)?.Value);
        
        protected Uri GetAbsoluteUri(string relativeUrl) => new Uri($"{Request.Scheme}://{Request.Host}{relativeUrl}", UriKind.Absolute);
    }
}