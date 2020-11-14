using System;
using System.Web;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Store.Common.Helpers;
using Store.WebAPI.Identity;
using Store.Models.Api.Identity;
using Store.Model.Common.Models.Identity;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

// Good blog post about two factor authentication: https://chsakell.com/2019/08/18/asp-net-core-identity-series-two-factor-authentication/
namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("two-factor")]
    public class TwoFactorController : IdentityControllerBase
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationAuthManager _authManager;
        private readonly SignInManager<IUser> _signInManager;
        private readonly ILogger _logger;

        public TwoFactorController
        (
            ApplicationUserManager userManager,
            ApplicationAuthManager authManager,
            SignInManager<IUser> signInManager,
            ILogger<AccountController> logger
        )
        : base(authManager, logger)
        {
            _userManager = userManager;
            _authManager = authManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>Retrieves user profile for the currently logged in user.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("user-profile")]
        public async Task<IActionResult> GetUserProfileAsync()
        {
            IUser user = await GetLoggedInUserAsync();
            IList<UserLoginInfo> logins = await _userManager.GetLoginsAsync(user);

            UserProfileGetApiModel userProfileResponse = new UserProfileGetApiModel
            {
                Username = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                ExternalLogins = logins.Select(login => login.ProviderDisplayName).ToList(),
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                TwoFactorClientRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };

            return Ok(userProfileResponse);
        }

        /// <summary>Generates or retrieves authenticator key for the currently logged in user.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("authenticator-key")]
        public async Task<IActionResult> GetUserAuthenticatorKeyAsync()
        {
            IUser user = await GetLoggedInUserAsync();

            string authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(authenticatorKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);

                _logger.LogInformation("A new authenticator key is generated.");
            }
            else
            {
                _logger.LogInformation("The existing authenticator key is retrieved from the database.");
            }

            AuthenticatorKeyGetApiModel authenticatorDetailsResponse = new AuthenticatorKeyGetApiModel
            {
                SharedKey = authenticatorKey,
                AuthenticatorUri = GenerateAuthenticatorUri(user.Email, authenticatorKey)
            };

            return Ok(authenticatorDetailsResponse);
        }

        /// <summary>Verifies the authenticator code for the currently logged in user. If successful, two factor authentication will be enabled.</summary>
        /// <param name="code">The authenticator code.</param>
        /// <returns>
        ///   Ten two factor recovery codes.
        /// </returns>
        [HttpPost]
        [Authorize]
        [Route("verify-authenticator-code")]
        public async Task<IActionResult> VerifyUserAuthenticatorCodeAsync([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Verification Code is missing.");
            }

            IUser user = await GetLoggedInUserAsync();
            bool isTwoFactorTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);

            if (isTwoFactorTokenValid)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);

                _logger.LogInformation("Two factor authentication is enabled for the user.");
            }
            else
            {
                return BadRequest("Verification Code is invalid.");
            }

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

                _logger.LogInformation("Ten two factor recovery codes are generatied for the user.");

                TwoFactoryRecoveryResponseApiModel response = new TwoFactoryRecoveryResponseApiModel
                {
                    RecoveryCodes = recoveryCodes.ToArray()
                };

                return Ok(response);
            }

            return NoContent();
        }

        /// <summary>
        /// Generates two factor recovery codes for the currently logged in user.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("generate-recovery-codes")]
        public async Task<IActionResult> GenerateNewRecoveryCodesAsync([FromQuery] int number)
        {
            IUser user = await GetLoggedInUserAsync();

            if (await _userManager.CountRecoveryCodesAsync(user) != 0)
            {
                return BadRequest("Cannot generate new recovery codes as old ones have not been used.");
            }

            IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, number);

            TwoFactoryRecoveryResponseApiModel response = new TwoFactoryRecoveryResponseApiModel
            {
                RecoveryCodes = recoveryCodes.ToArray()
            };

            return Ok(response);
        }

        /// <summary>Disables the two factor authentication for the currently logged in user.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Authorize]
        [Route("disable")]
        public async Task<IActionResult> DisableAsync()
        {
            IUser user = await GetLoggedInUserAsync();

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return BadRequest("Cannot disable two factor authentication as it's not currently enabled.");
            }

            IdentityResult result = await _userManager.SetTwoFactorEnabledAsync(user, false);

            _logger.LogInformation("Two factor authentication is disabled for the user.");

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        /// <summary>Authenticates the user using the two factor authentication code.</summary>
        /// <param name="authenticateModel">The authenticate model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticateTwoFactorRequestApiModel authenticateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify client information
            if (!Guid.TryParse(authenticateModel.ClientId, out Guid clientId) || GuidHelper.IsNullOrEmpty(clientId))
            {
                return BadRequest($"Client '{clientId}' format is invalid.");
            }

            IUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return BadRequest("Two-factor authentication action is not supported for the user.");
            }

            SignInResult signInResult;
            if (!authenticateModel.UseRecoveryCode)
            {
                //Note: isPersistent, rememberClient: false - no need to store browser cookies in Web API.
                signInResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticateModel.Code, false, false);
            }
            else
            {
                signInResult = await _signInManager.TwoFactorRecoveryCodeSignInAsync(authenticateModel.Code);
            }

            return await AuthenticateAsync(signInResult, user, clientId);
        }

        #region Helpers

        private async Task<IUser> GetLoggedInUserAsync()
        {
            string userName = User.FindFirst(ClaimTypes.Name).Value;
            IUser user = await _userManager.FindByNameAsync(userName);

            return user;
        }

        private string GenerateAuthenticatorUri(string email, string authenticatorKey)
        {
            const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

            return string.Format
            (
                AuthenticatorUriFormat,
                HttpUtility.UrlPathEncode("ASP.NET Core Identity"),
                HttpUtility.UrlPathEncode(email),
                authenticatorKey
            );
        }

        #endregion
    }
}