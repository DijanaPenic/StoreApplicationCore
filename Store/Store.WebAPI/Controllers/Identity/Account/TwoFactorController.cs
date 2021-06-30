using System;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Store.Common.Enums;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Infrastructure.Authorization.Extensions;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Controllers
{
    public partial class UserController
    {
        /// <summary>Gets the authenticator key for the user. Authenticator key will be retrieved from the database or created (if none is found).</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("{userId:guid}/two-factor/authenticator")]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserAuthenticatorKeyAsync([FromRoute] Guid userId)
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

            string authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(authenticatorKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user); // This will set a new AuthenticatorKey

                authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user); // Now we can retrieve the new key
                if (string.IsNullOrEmpty(authenticatorKey))
                {
                    return InternalServerError();
                }

                _logger.LogInformation("A new authenticator key is generated.");
            }
            else
            {
                _logger.LogInformation("The existing authenticator key is retrieved from the database.");
            }
            
            string authenticatorUri = string.Format
            (
                "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                HttpUtility.UrlPathEncode("ASP.NET Core Identity"),
                HttpUtility.UrlPathEncode(user.Email),
                authenticatorKey
            );

            AuthenticatorKeyGetApiModel authenticatorDetailsResponse = new AuthenticatorKeyGetApiModel
            {
                SharedKey = authenticatorKey,
                AuthenticatorUri = authenticatorUri
            };

            return Ok(authenticatorDetailsResponse);
        }
        
        /// <summary>
        /// Generates new two-factor recovery codes for the user. New recovery codes can be issued only if there are no recovery codes in the system for the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="number">The number.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("{userId:guid}/two-factor/recovery-codes")]
        [Produces("application/json")]
        public async Task<IActionResult> GenerateNewRecoveryCodesAsync([FromRoute] Guid userId, [FromQuery] int number)
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

            if (await _userManager.CountRecoveryCodesAsync(user) != 0)
            {
                return BadRequest("Cannot generate new recovery codes as old ones have not been used.");
            }

            IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, number);

            TwoFactorRecoveryResponseApiModel response = new()
            {
                RecoveryCodes = recoveryCodes.ToArray()
            };

            return Ok(response);
        }

        /// <summary>Verifies the authenticator code for the user. If successful, two-factor authentication will be enabled.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="code">The authenticator code.</param>
        /// <returns>
        ///   Ten two-factor recovery codes.
        /// </returns>
        [HttpPut]
        [Authorize]
        [Route("{userId:guid}/two-factor/actions/enable")]
        [Produces("application/json")]
        public async Task<IActionResult> VerifyUserAuthenticatorCodeAsync([FromRoute] Guid userId, [FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Verification Code is missing.");
            }
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

            bool isTwoFactorTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);

            if (isTwoFactorTokenValid)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);

                _logger.LogInformation("Two-factor authentication is enabled for the user.");
            }
            else
            {
                return BadRequest("Verification Code is invalid.");
            }

            if (await _userManager.CountRecoveryCodesAsync(user) != 0) return NoContent();
            
            IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            _logger.LogInformation("Ten two-factor recovery codes are generated for the user.");

            TwoFactorRecoveryResponseApiModel response = new TwoFactorRecoveryResponseApiModel
            {
                RecoveryCodes = recoveryCodes.ToArray()
            };

            return Ok(response);

        }

        /// <summary>Disables the two-factor authentication for the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPut]
        [Authorize]
        [Route("{userId:guid}/two-factor/actions/disable")]
        public async Task<IActionResult> DisableUserTwoFactorAsync([FromRoute] Guid userId)
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

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return BadRequest("Cannot disable two-factor authentication as it's not currently enabled.");
            }

            IdentityResult result = await _userManager.SetTwoFactorEnabledAsync(user, false);

            _logger.LogInformation("Two-factor authentication is disabled for the user.");

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }
    }
}