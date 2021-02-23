using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Store.Services.Identity;
using Store.WebAPI.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.WebAPI.Constants;
using Store.Model.Common.Models;
using Store.WebAPI.Models;

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


        /// <summary>Retrieves role access claims by specified search criteria.</summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="isDescendingSortOrder">if set to <c>true</c> [is descending sort order].</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("{roleId:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid roleId, string searchString = DefaultParameters.SearchString, int pageNumber = DefaultParameters.PageNumber, 
                                                  int pageSize = DefaultParameters.PageSize, bool isDescendingSortOrder = DefaultParameters.IsDescendingSortOrder)
        {
            if (roleId == Guid.Empty)
                return BadRequest();

            IRole role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return NotFound();
            }

            IPagedEnumerable<IRoleClaim> roleClaims = await _roleManager.FindClaimsAsync
            (
                ApplicationPermissionsManager.CLAIM_KEY,
                searchString,
                isDescendingSortOrder,
                pageNumber,
                pageSize,
                role
            );

            if (roleClaims != null)
            {
                return Ok(_mapper.Map<PagedApiResponse<RoleClaimGetApiModel>>(roleClaims));
            }

            return NoContent();
        }
    }
}