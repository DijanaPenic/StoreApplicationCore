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
    abstract public class ApplicationControllerBase : ControllerBase
    {
        private IOptionsFactory _optionsFactory = null;
        private IPagingFactory _pagingFactory = null;
        private ISortingFactory _sortingFactory = null;
        private IFilteringFactory _filteringFactory = null;
        private readonly IQueryUtilityFacade _queryUtilityFacade;

        protected IOptionsFactory OptionsFactory
        {
            get
            {
                if (_optionsFactory == null)
                {
                    _optionsFactory = _queryUtilityFacade.CreateOptionsFactory();
                }
                return _optionsFactory;
            }
        }

        protected IFilteringFactory FilteringFactory
        {
            get
            {
                if (_filteringFactory == null)
                {
                    _filteringFactory = _queryUtilityFacade.CreateFilteringFactory();
                }
                return _filteringFactory;
            }
        }

        protected IPagingFactory PagingFactory
        {
            get
            {
                if (_pagingFactory == null)
                {
                    _pagingFactory = _queryUtilityFacade.CreatePagingFactory();
                }
                return _pagingFactory;
            }
        }

        protected ISortingFactory SortingFactory
        {
            get
            {
                if (_sortingFactory == null)
                {
                    _sortingFactory = _queryUtilityFacade.CreateSortingFactory();
                }
                return _sortingFactory;
            }
        }

        public ApplicationControllerBase(IQueryUtilityFacade queryUtilityFacade)
        {
            _queryUtilityFacade = queryUtilityFacade;
        }

        protected IActionResult InternalServerError() => StatusCode(StatusCodes.Status500InternalServerError);

        protected IActionResult Created() => StatusCode(StatusCodes.Status201Created);

        protected bool IsCurrentUser(Guid userId) => GetUserId(User) == userId;

        protected Guid GetCurrentUserClientId() => GetClientId(User);

        protected bool IsUser(Guid userId, ClaimsPrincipal claimsPrincipal) => GetUserId(claimsPrincipal) == userId;

        protected Guid GetUserId(ClaimsPrincipal claimsPrincipal) => Guid.Parse(claimsPrincipal.Claims.FirstOrDefault(uc => uc.Type == ClaimTypes.NameIdentifier)?.Value);

        protected Guid GetClientId(ClaimsPrincipal claimsPrincipal) => Guid.Parse(claimsPrincipal.Claims.FirstOrDefault(uc => uc.Type == ApplicationClaimTypes.ClientIdentifier)?.Value);
        
        protected Uri GetAbsoluteUri(string relativeUrl) => new Uri($"{Request.Scheme}://{Request.Host}{relativeUrl}", UriKind.Absolute);
    }
}