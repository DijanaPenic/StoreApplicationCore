using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using Store.Common.Enums;
using Store.Common.Extensions;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Infrastructure.Authorization.Extensions;
using Store.WebAPI.Infrastructure.Authorization.Attributes;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Controllers
{
    public partial class UserController
    {
        /// <summary>Retrieves external login connections for the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("{userId:guid}/external-login")]
        [Produces("application/json")]
        public async Task<IActionResult> GetExternalLoginAsync([FromRoute] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            bool hasPermissions = IsCurrentUser(userId) || (await _authorizationService.AuthorizeAsync(User, SectionType.User, AccessType.Full)).Succeeded;
            if (!hasPermissions)
            {
                return Forbid();
            }

            IUser user = await _userManager.FindUserByKeyAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            IList<UserLoginInfo> logins = await _userManager.FindLoginsAsync(user, true);

            return Ok(_mapper.Map<IList<ExternalLoginGetApiModel>>(logins));
        }

        /// <summary>
        /// Removes social external connection between user and provider.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="name">The login provider name.</param>
        /// <param name="key">The login provider key.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpDelete]
        [Authorize]
        [Route("{userId:guid}/external-login")]
        public async Task<IActionResult> DisconnectExternalLoginAsync([FromRoute] Guid userId, [FromQuery] string name, [FromQuery] string key)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }
            if(string.IsNullOrEmpty(name))
            {
                return BadRequest("Login Provider cannot be empty.");
            }
            if(string.IsNullOrEmpty(key))
            {
                return BadRequest("Provider Key cannot be empty.");
            }

            bool hasPermissions = IsCurrentUser(userId) || (await _authorizationService.AuthorizeAsync(User, SectionType.User, AccessType.Full)).Succeeded;
            if (!hasPermissions)
            {
                return Forbid();
            }

            IUser user = await _userManager.FindUserByKeyAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult removeLoginResult = await _userManager.RemoveLoginAsync(user, name, key);

            return removeLoginResult.Succeeded ? Ok() : BadRequest(removeLoginResult.Errors);
        }
        
        /// <summary>Confirms the external provider association.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPut] 
        [ClientAuthorization]
        [Route("{userId:guid}/external-login/actions/verify")]
        public async Task<IActionResult> ConfirmExternalProviderAsync([FromRoute] Guid userId, [FromQuery] string token)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Token is required.");
            }

            IUser user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            string decodedToken = token.Base64Decode();

            if (!user.EmailConfirmed)
            {
                // External provider is authenticated source so we can confirm the email
                IdentityResult emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (!emailConfirmationResult.Succeeded) return BadRequest(emailConfirmationResult.Errors);
            }

            // Create a new external login for the user
            IdentityResult confirmLoginResult = await _userManager.ConfirmLoginAsync(user, decodedToken);

            if (!confirmLoginResult.Succeeded) return BadRequest(confirmLoginResult.Errors);

            return Ok();
        }
    }
}