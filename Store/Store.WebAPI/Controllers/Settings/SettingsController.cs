using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Store.Cache.Common;
using Store.WebAPI.Constants;
using Store.WebAPI.Models.Settings;
using Store.Common.Parameters;
using Store.Model.Common.Models;
using Store.Service.Common.Services;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/settings")]
    public class SettingsController : ApplicationControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ICountriesService _countriesService;
        private readonly ICacheProvider _cacheProvider;

        public SettingsController
        (
            ILogger<SettingsController> logger,
            IMapper mapper,
            ICountriesService countriesService,
            ICacheManager cacheManager,
            IQueryUtilityFacade queryUtilityFacade
        ) : base(queryUtilityFacade)
        {
            _logger = logger;
            _mapper = mapper;
            _countriesService = countriesService;
            _cacheProvider = cacheManager.CacheProvider;
        }

        /// <summary>Retrieves the countries lookup from database or cache.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("countries")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCountriesAsync()
        {
            IList<ICountry> countries = await _cacheProvider.GetOrAddAsync
            (
                CacheParameters.Keys.AllCountries,
                () => _countriesService.GetCountriesAsync(),
                DateTimeOffset.MaxValue,
                CacheParameters.Groups.Settings
            );

            return countries == null ? InternalServerError() : Ok(_mapper.Map<IList<CountryGetApiModel>>(countries));
        }
    }
}