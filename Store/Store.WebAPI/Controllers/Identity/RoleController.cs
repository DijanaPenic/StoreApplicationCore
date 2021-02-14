using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Store.Cache.Common;
using Store.WebAPI.Constants;
using Store.WebAPI.Models.Identity;
using Store.Services.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RoleController : ApplicationControllerBase
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ApplicationRoleManager _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RoleController
        (
            ApplicationRoleManager roleManager,
            ICacheManager cacheManager,
            IMapper mapper,
            ILogger<RoleController> logger
        )
        {
            _roleManager = roleManager;
            _cacheProvider = cacheManager.CacheProvider;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>Retrieves the role by identifier.</summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("{roleId:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid roleId)
        {
            if (roleId == Guid.Empty)
                return BadRequest();

            IRole role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role != null)
                return Ok(_mapper.Map<RoleGetApiModel>(role));

            return NotFound();
        }

        /// <summary>Creates a new role.</summary>
        /// <param name="roleModel">The role model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> PostAsync([FromBody] RolePostApiModel roleModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            IRole role = _mapper.Map<IRole>(roleModel);

            IdentityResult roleResult = await _roleManager.CreateAsync(role);

            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            _logger.LogInformation("A new role has been created successfully.");

            return Ok();
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
        public async Task<IActionResult> PatchAsync([FromRoute] Guid roleId, [FromBody] RolePatchApiModel roleModel)
        {
            if (roleId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            if (!ModelState.IsValid)
                return BadRequest();

            IRole role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return NotFound();
            }

            // Changing role to "not stackable" so need to verify if some users are associated with more than one role
            if(!roleModel.Stackable && role.Stackable)
            {
                int roleCount = await _roleManager.GetUserRoleCombinationCountAsync(role);
                if(roleCount > 1)
                {
                    return BadRequest("Cannot change role to 'unstackable' as there are some users that use this role combined with other roles.");
                }
            }

            _mapper.Map(roleModel, role);

            IdentityResult roleResult = await _roleManager.UpdateAsync(role);

            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            _logger.LogInformation("Role has been updated successfully.");

            return Ok();
        }

        /// <summary>Deletes the role.</summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpDelete]
        [Route("{roleId:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid roleId)
        {
            if (roleId == Guid.Empty)
                return BadRequest();

            IRole role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return NotFound();
            }

            int userCount = await _roleManager.GetUserCountByRoleAsync(role);
            if(userCount > 0)
            {
                return BadRequest($"Unable to delete role - role is associated with {userCount} user accounts.");
            }

            IdentityResult roleResult = await _roleManager.DeleteAsync(role);

            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            _logger.LogInformation("Role has been deleted successfully.");

            return Ok();
        }
    }
}