using System;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using Resta.UriTemplates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Store.Common.Extensions;
using Store.Services.Identity;
using Store.WebAPI.Models.Identity;
using Store.Messaging.Services.Common;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/recover-password")]
    public class RecoverPasswordController : IdentityControllerBase
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ILogger _logger;
        private readonly IEmailSenderService _emailClientSender;

        public RecoverPasswordController
        (
            ApplicationUserManager userManager,
            ILogger<RegisterController> logger,
            IEmailSenderService emailClientSender
        )
        {
            _userManager = userManager;
            _logger = logger;
            _emailClientSender = emailClientSender;
        }

        /// <summary>Initiates a password recovery process for the specified user.</summary>
        /// <param name="passwordRecoveryModel">The forgot password model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("")]
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

            await _emailClientSender.SendResetPasswordEmailAsync(passwordRecoveryModel.Email, callbackUrl, user.UserName);

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

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }
    }
}