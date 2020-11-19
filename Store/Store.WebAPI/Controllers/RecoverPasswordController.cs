using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using Store.WebAPI.Identity;
using Store.Common.Extensions;
using Store.Models.Api.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("recover-password")]
    public class RecoverPasswordController : IdentityControllerBase
    {
        private readonly ApplicationUserManager _userManager;

        public RecoverPasswordController
        (
            ApplicationUserManager userManager
        )
        {
            _userManager = userManager;
        }

        /// <summary>Initiates a password recovery process for the specified user.</summary>
        /// <param name="email">The user's email.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("")]
        public async Task<IActionResult> InitiatePasswordRecoveryAsync([FromQuery]string email)
        {
            if(string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is missing.");
            }

            IUser user = await _userManager.FindByEmailAsync(email);

            // User with verified email cannot be found, so return NotFound
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return NotFound();
            }

            // Get forgot password confirmation token
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            TokenResponseApiModel registerResponse = new TokenResponseApiModel
            {
                UserId = user.Id,
                ConfirmationToken = token.Base64ForUrlEncode()
            };

            return Ok(registerResponse);
        }

        /// <summary>Updates the user's password.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="forgotPasswordModel">The forgot password model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [AllowAnonymous]
        [Route("{userId:guid}")]
        public async Task<IActionResult> ResetUserPasswordAsync([FromRoute]Guid userId, ResetPasswordPostApiModel forgotPasswordModel) // TODO - potentially move token from header to model
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string token = Request.Headers["Token"].First();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing.");
            }

            IUser user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, token.Base64ForUrlDecode(), forgotPasswordModel.Password);

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }
    }
}