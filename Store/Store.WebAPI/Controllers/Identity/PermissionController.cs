﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Store.WebAPI.Models;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Constants;
using Store.Services.Identity;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Common.Helpers;
using Store.Common.Enums;

namespace Store.WebAPI.Controllers.Identity
{
    [ApiController]
    [Route("api/permissions")]
    public class PermissionController : ApplicationControllerBase
    {
        private readonly ApplicationRoleManager _roleManager;
        private readonly ApplicationPermissionsManager _permissionManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PermissionController
        (
            ApplicationRoleManager roleManager,
            ApplicationPermissionsManager permissionsManager,
            IMapper mapper,
            ILogger<PermissionController> logger
        )
        {
            _roleManager = roleManager;
            _permissionManager = permissionsManager;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>Updates access policy.</summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="policyModel">The policy model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Route("{roleId:guid}")]
        [Consumes("application/json")]
        public async Task<IActionResult> PostAsync([FromRoute] Guid roleId, [FromBody] PolicyPostApiModel policyModel)
        {
            if (roleId == Guid.Empty)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            IRole role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return NotFound();
            }

            IPolicy policy = _mapper.Map<IPolicy>(policyModel);
            IdentityResult updatePolicyResult = await _permissionManager.UpdatePolicyAsync(role, policy);

            if (!updatePolicyResult.Succeeded) return BadRequest(updatePolicyResult.Errors);

            _logger.LogInformation("A new policy has been updated successfully.");

            return Ok();
        }

        /// <summary>Retrieves role with policies for a certain section by specified search criteria.</summary>
        /// <param name="section">The section.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="isDescendingSortOrder">if set to <c>true</c> [is descending sort order].</param>
        /// <param name="sortOrderProperty">The sort order property.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("{section}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync([FromRoute] string section, [FromQuery] string searchString = DefaultParameters.SearchString, [FromQuery] int pageNumber = DefaultParameters.PageNumber, 
                                                  [FromQuery] int pageSize = DefaultParameters.PageSize, [FromQuery] bool isDescendingSortOrder = DefaultParameters.IsDescendingSortOrder, 
                                                  [FromQuery] string sortOrderProperty = nameof(RoleGetApiModel.Name))
        {
            if(!Enum.TryParse(section, out SectionType sectionType))
            {
                return BadRequest("Section not supported");
            }

            IPagedEnumerable<IRole> roles = await _roleManager.FindRolesWithPoliciesAsync
            (
                sectionType,
                searchString,
                ModelMapperHelper.GetPropertyMapping<RoleGetApiModel, IRole>(_mapper, sortOrderProperty),
                isDescendingSortOrder,
                pageNumber,
                pageSize
            );

            if (roles != null)
            {
                return Ok(_mapper.Map<PagedApiResponse<RoleGetApiModel>>(roles));
            }

            return NoContent();
        }
    }
}