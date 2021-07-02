using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Resta.UriTemplates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Store.Common.Enums;
using Store.Common.Extensions;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Infrastructure.Authorization.Extensions;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Controllers
{
    public partial class UserController
    {
        /// <summary>Retrieves user's profile information.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("{userId:guid}/profile")]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserProfileAsync([FromRoute] Guid userId)
        {
            bool hasPermissions = IsCurrentUser(userId) || (await _authorizationService.AuthorizeAsync(User, SectionType.User, AccessType.Full)).Succeeded;
            if (!hasPermissions)
            {
                return Forbid();
            }

            IUser user = await _userManager.FindUserByKeyAsync(userId);
            if (user == null)
            {
                return NotFound("User cannot be found.");
            }

            IList<UserLoginInfo> logins = await _userManager.FindLoginsAsync(user, true);

            UserProfileGetApiModel userProfileResponse = new()
            {
                Username = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                ExternalLogins = _mapper.Map<ExternalLoginGetApiModel[]>(logins),
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                TwoFactorClientRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };

            return Ok(userProfileResponse);
        }

        /// <summary>Updates the user's profile.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userProfileModel">The user profile model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Authorize]
        [Route("{userId:guid}/profile")]
        [Produces("application/json")]
        public async Task<IActionResult> PatchUserProfileAsync([FromRoute] Guid userId, [FromBody] UserProfilePatchApiModel userProfileModel)
        {
            if (!IsCurrentUser(userId))
            {
                return Forbid();
            }

            IUser user = await _userManager.FindUserByKeyAsync(userId);
            if (user == null)
            {
                return NotFound("User cannot be found.");
            }

            if (user.Email != userProfileModel.Email) user.EmailConfirmed = false;

            _mapper.Map(userProfileModel, user);
            IdentityResult userProfileResult = await _userManager.UpdateAsync(user);

            if (!userProfileResult.Succeeded) return BadRequest(userProfileResult.Errors);
            
            // Need to send email confirmation if email is not confirmed
            // Note: phone number cannot be updated
            if (!user.EmailConfirmed)
            {
                // Get email confirmation token
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                _logger.LogInformation("Email confirmation token has been generated.");

                UriTemplate template = new(userProfileModel.ConfirmationUrl);
                string callbackUrl = template.Resolve(new Dictionary<string, object>
                {
                    { "userId", user.Id.ToString() },
                    { "token", token.Base64Encode() }
                });

                _logger.LogInformation("Sending email confirmation email.");

                await _emailService.SendConfirmEmailAsync(GetCurrentUserClientId(), user.Email, callbackUrl, user.UserName);
            }

            return NoContent();
        }
    }
}