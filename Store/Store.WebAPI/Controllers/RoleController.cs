using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using Store.Cache.Common;
using Store.WebAPI.Identity;
using Store.WebAPI.Constants;
using Store.WebAPI.Infrastructure;
using Store.Models.Api.Identity;
using Store.Model.Common.Models.Identity;
using Store.Common.Helpers.Identity;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RoleController : ApplicationControllerBase
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ApplicationRoleManager _roleManager;
        private readonly IMapper _mapper;

        public RoleController
        (
            ApplicationRoleManager roleManager,
            ICacheManager cacheManager,
            IMapper mapper
        )
        {
            _roleManager = roleManager;
            _cacheProvider = cacheManager.CacheProvider;
            _mapper = mapper;
        }

        /// <summary>Retrieves all roles.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("")]
        [AuthorizationFilter(RoleHelper.Admin)]
        public async Task<IActionResult> GetRolesAsync()
        {
            IEnumerable<IRole> roles = await _cacheProvider.GetOrAddAsync
            (
                CacheParameters.Keys.AllRoles,
                _roleManager.GetRolesAsync,
                DateTimeOffset.MaxValue,
                CacheParameters.Groups.Identity
            );

            return Ok(_mapper.Map<IEnumerable<RoleGetApiModel>>(roles));
        }
    }
}