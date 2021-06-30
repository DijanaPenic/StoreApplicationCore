using Resta.UriTemplates;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Store.Common.Extensions;
using Store.Common.Parameters;
using Store.Services.Identity;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Infrastructure.Authorization.Attributes;
using Store.Messaging.Services.Common;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/password-recovery")]
    public class PasswordRecoveryController : ApplicationControllerBase
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger _logger;

        public PasswordRecoveryController
        (
            ApplicationUserManager userManager,
            IEmailService emailService,
            ILogger<PasswordRecoveryController> logger,
            IQueryUtilityFacade queryUtilityFacade
        ) : base(queryUtilityFacade)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>Initiates a password recovery process for the specified user.</summary>
        /// <param name="passwordRecoveryModel">The forgot password model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [ClientAuthorization]
        [Consumes("application/json")]
        public async Task<IActionResult> InitiatePasswordRecoveryAsync([FromBody] PasswordRecoveryPostApiModel passwordRecoveryModel)
        {
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
                { "token", token.Base64Encode() }
            });

            _logger.LogInformation("Sending password recovery email.");

            await _emailService.SendResetPasswordAsync(GetCurrentUserClientId(), passwordRecoveryModel.Email, callbackUrl, user.UserName);

            return Ok();
        }

        /// <summary>Updates the user's password.</summary>
        /// <param name="passwordRecoveryModel">The forgot password model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPut]
        [ClientAuthorization]
        [Consumes("application/json")]
        public async Task<IActionResult> ResetUserPasswordAsync([FromBody] PasswordRecoveryPutApiModel passwordRecoveryModel)
        {
            IUser user = await _userManager.FindByIdAsync(passwordRecoveryModel.UserId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, passwordRecoveryModel.PasswordRecoveryToken.Base64Decode(), passwordRecoveryModel.NewPassword);

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }
    }
}