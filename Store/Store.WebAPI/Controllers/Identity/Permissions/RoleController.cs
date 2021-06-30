﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Common.Parameters;
using Store.Common.Parameters.Filtering;
using Store.WebAPI.Models;
using Store.WebAPI.Constants;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Infrastructure.Authorization.Attributes;
using Store.Services.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RoleController : ApplicationControllerBase
    {
        private readonly ApplicationRoleManager _roleManager;
        private readonly ApplicationPermissionsManager _permissionManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RoleController
        (
            ApplicationRoleManager roleManager,
            ApplicationPermissionsManager permissionsManager,
            IMapper mapper,
            ILogger<RoleController> logger,
            IQueryUtilityFacade queryUtilityFacade
        ) : base(queryUtilityFacade)
        {
            _roleManager = roleManager;
            _permissionManager = permissionsManager;
            _mapper = mapper;
            _logger = logger;
        }
        
        /// <summary>Creates a new role.</summary>
        /// <param name="roleModel">The role model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Consumes("application/json")]
        [SectionAuthorization(SectionType.Role, AccessType.Create)]
        public async Task<IActionResult> PostAsync([FromBody] RolePostApiModel roleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IRole role = _mapper.Map<IRole>(roleModel);

            IdentityResult roleResult = await _roleManager.CreateAsync(role);

            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            _logger.LogInformation("A new role has been created successfully.");

            return Created();
        }

        /// <summary>Retrieves roles by specified search criteria.</summary>
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
        [SectionAuthorization(SectionType.Role, AccessType.Read)]
        public async Task<IActionResult> GetAsync([FromQuery] string includeProperties = DefaultParameters.IncludeProperties,
                                                  [FromQuery] string searchString = DefaultParameters.SearchString,
                                                  [FromQuery] int pageNumber = DefaultParameters.PageNumber,
                                                  [FromQuery] int pageSize = DefaultParameters.PageSize,
                                                  [FromQuery] string sortOrder = DefaultParameters.SortOrder)
        {
            IPagedList<IRole> roles = await _roleManager.FindRolesAsync
            (
                filter: FilteringFactory.Create<IFilteringParameters>(searchString),
                paging: PagingFactory.Create(pageNumber, pageSize),
                sorting: SortingFactory.Create(ModelMapperHelper.GetSortPropertyMappings<RoleGetApiModel, IRole>(_mapper, sortOrder)),
                options: OptionsFactory.Create(ModelMapperHelper.GetPropertyMappings<RoleGetApiModel, IRole>(_mapper, includeProperties))
            );

            if (roles != null)
            {
                return Ok(_mapper.Map<PagedApiResponse<RoleGetApiModel>>(roles));
            }

            return NoContent();
        }
        
        /// <summary>Retrieves the role by identifier.</summary>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("{roleId:guid}")]
        [Produces("application/json")]
        [SectionAuthorization(SectionType.Role, AccessType.Read)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid roleId, [FromQuery] string includeProperties = DefaultParameters.IncludeProperties)
        {
            if (roleId == Guid.Empty)
            {
                return BadRequest("Role Id cannot be empty.");
            }

            IRole role = await _roleManager.FindRoleByIdAsync(roleId, OptionsFactory.Create(ModelMapperHelper.GetPropertyMappings<RoleGetApiModel, IRole>(_mapper, includeProperties)));
         if (role != null)
                return Ok(_mapper.Map<RoleGetApiModel>(role));

            return NotFound();

           }

        /// <summary>Updates the role.</summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="roleModel">The role model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Consumes("application/json")]
        [Route("{roleId:guid}")]
        [SectionAuthorization(SectionType.Role, AccessType.Update)]
        public async Task<IActionResult> PatchAsync([FromRoute] Guid roleId, [FromBody] RolePatchApiModel roleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (roleId == Guid.Empty)
            {
                return BadRequest("Role Id cannot be empty.");
            }

            IRole role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return NotFound();
            }

            // Changing role to "not stackable" so need to verify if some users are associated with more than one role
            if(!roleModel.Stackable && role.Stackable)
            {
                int userCount = await _roleManager.GetCountByRoleNameAsync(role);
                if(userCount > 1)
                {
                    return BadRequest("Role cannot be stacked as there are some users that use this role combined with other roles.");
                }
            }

            _mapper.Map(roleModel, role);

            IdentityResult roleResult = await _roleManager.UpdateAsync(role);

            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            _logger.LogInformation("Role has been updated successfully.");

            return Ok();
        }
    }
}