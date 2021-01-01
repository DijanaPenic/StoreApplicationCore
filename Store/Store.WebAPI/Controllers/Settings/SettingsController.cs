using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Store.Cache.Common;
using Store.WebAPI.Constants;
using Store.Model.Common.Models;
using Store.Service.Common.Services;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/settings")]
    public class SettingsController : ApplicationControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICountriesService _countriesService;
        private readonly ICacheProvider _cacheProvider;

        public SettingsController
        (
            ILogger<SettingsController> logger,
            ICountriesService countriesService,
            ICacheManager cacheManager
        )
        {
            _logger = logger;
            _countriesService = countriesService;
            _cacheProvider = cacheManager.CacheProvider;
        }

        /// <summary>Retrieves the countries lookup.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("countries/all")]
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

            if(countries == null)
            {
                return InternalServerError();
            }

            return Ok(countries);
        }
    }
}