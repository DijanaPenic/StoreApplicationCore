using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Store.Services.Identity;
using Store.WebAPI.Models.Identity;
using Store.Model.Common.Models.Identity;

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
    }
}