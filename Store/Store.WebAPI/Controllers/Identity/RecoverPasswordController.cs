using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Resta.UriTemplates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Store.Common.Extensions;
using Store.Services.Identity;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Infrastructure.Authorization.Attributes;
using Store.Messaging.Services.Common;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/recover-password")]
    public class RecoverPasswordController : ApplicationControllerBase
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IEmailSenderService _emailClientSender;
        private readonly ILogger _logger;

        public RecoverPasswordController
        (
            ApplicationUserManager userManager,
            IEmailSenderService emailClientSender,
            ILogger<RegisterController> logger
        )
        {
            _userManager = userManager;
            _emailClientSender = emailClientSender;
            _logger = logger;
        }

        /// <summary>Initiates a password recovery process for the specified user.</summary>
        /// <param name="passwordRecoveryModel">The forgot password model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [ClientAuthorization]
        [Route("")]
        [Consumes("application/json")]
        public async Task<IActionResult> InitiatePasswordRecoveryAsync(PasswordRecoveryPostApiModel passwordRecoveryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IUser user = await _userManager.FindByEmailAsync(passwordRecoveryModel.Email);

            // User with verified email cannot be found, so return NotFound
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return NotFound();
            }

            // Get forgot password confirmation token
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            _logger.LogInformation("Password recovery token has been generated.");

            UriTemplate template = new UriTemplate(passwordRecoveryModel.ConfirmationUrl);
            string callbackUrl = template.Resolve(new Dictionary<string, object>
            {
                { "userId", user.Id.ToString() },
                { "token", token.Base64ForUrlEncode() }
            });

            _logger.LogInformation("Sending password recovery email.");

            await _emailClientSender.SendResetPasswordAsync(GetCurrentUserClientId(), passwordRecoveryModel.Email, callbackUrl, user.UserName);

            return Ok();
        }

        /// <summary>Updates the user's password.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="passwordRecoveryModel">The forgot password model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [AllowAnonymous]
        [Route("{userId:guid}")]
        [Consumes("application/json")]
        public async Task<IActionResult> ResetUserPasswordAsync([FromRoute]Guid userId, PasswordRecoveryPatchApiModel passwordRecoveryModel)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IUser user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, passwordRecoveryModel.PasswordRecoveryToken.Base64ForUrlDecode(), passwordRecoveryModel.NewPassword);

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }
    }
}