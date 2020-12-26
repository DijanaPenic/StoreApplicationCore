using System;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using X.PagedList;
using Resta.UriTemplates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Store.Common.Helpers;
using Store.Common.Helpers.Identity;
using Store.Common.Extensions;
using Store.Services.Identity;
using Store.Service.Common.Services.Identity;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Infrastructure.Attributes;
using Store.Messaging.Services.Common;
using Store.Model.Common.Models.Identity;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/authenticate")]
    public class AuthenticateController : IdentityControllerBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationAuthManager _authManager;
        private readonly SignInManager<IUser> _signInManager;
        private readonly IEmailSenderService _emailClientSender;

        public AuthenticateController
        (
            ApplicationUserManager userManager,
            ApplicationAuthManager authManager,
            SignInManager<IUser> signInManager,
            ILogger<AuthenticateController> logger,
            IEmailSenderService emailClientSender
        )
        {
            _userManager = userManager;
            _authManager = authManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailClientSender = emailClientSender;
        }

        /// <summary>Retrieves the authentication info for the currently logged in user.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("info")]
        public async Task<IActionResult> AuthenticateInfoAsync()
        {
            AuthenticateInfoGetApiModel authInfoModel = new AuthenticateInfoGetApiModel
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Username = User.Identity.IsAuthenticated ? User.Identity.Name : string.Empty,
                ExternalLoginProvider = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationMethod)?.Value,
                DisplaySetPassword = User.Identity.IsAuthenticated && !(await _userManager.HasPasswordAsync((await _userManager.GetUserAsync(User))))
            };

            return Ok(authInfoModel);
        }

        /// <summary>Authenticates the user.</summary>
        /// <param name="authenticateModel">The authenticate model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateRequestApiModel authenticateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify client information
            if(!Guid.TryParse(authenticateModel.ClientId, out Guid clientId) || GuidHelper.IsNullOrEmpty(clientId))
            {
                return BadRequest($"Client '{clientId}' format is invalid."); 
            }

            string clientAuthResult = await _authManager.AuthenticateClientAsync(clientId, authenticateModel.ClientSecret);
            if(!string.IsNullOrEmpty(clientAuthResult))
            {
                return Unauthorized(clientAuthResult);
            }

            // Check user's status
            IUser user = await _userManager.FindByNameAsync(authenticateModel.UserName);
            if(user == null)
            {
                return Unauthorized($"Failed to log in - invalid username and/or password.");
            }
            if (user.IsDeleted)
            {
                return Unauthorized($"User [{user.UserName}] has been deleted.");
            }
            if (!user.IsApproved)
            {
                return Unauthorized($"User [{user.UserName}] is not approved.");
            }

            // Attempt to sign in
            // Note: CheckPasswordSignInAsync - this method doesn't perform 2fa check.
            // Note: PasswordSignInAsync - this method performs 2fa check, but also generates the ".AspNetCore.Identity.Application" cookie. Cookie creation cannot be disabled (SignInManager is heavily dependant on cookies - by design).
            // Note: isPersistent: false - no need to store browser cookies in Web API.
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, authenticateModel.Password, isPersistent: false, lockoutOnFailure: true); 
            
            return await AuthenticateAsync(signInResult, user, clientId);
        }

        /// <summary>Refreshes authentication tokens (refresh and access tokens).</summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Authorize]
        [Route("renew/token/{refreshToken}")]
        public async Task<IActionResult> RenewTokensAsync([FromRoute]string refreshToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retrieve client_id for the currently logged in user
            Guid clientId = GetCurrentUserClientId();

            try
            {
                // Generate new tokens
                string accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                IJwtAuthResult jwtResult = await _authManager.RenewTokensAsync(refreshToken, accessToken, clientId);

                _logger.LogInformation("New access and refresh tokens are generated for the user.");

                RenewTokenResponseApiModel authenticationResponse = new RenewTokenResponseApiModel
                {
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken
                };

                return Ok(authenticationResponse);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(ex.Message); 
            }
        }

        /// <summary>Deletes refresh tokens that have expired.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpDelete]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("expired-refresh-tokens")]
        public async Task<IActionResult> DeleteExpiredRefreshTokensAsync()
        {
            try
            {
                await _authManager.RemoveExpiredRefreshTokensAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Deletion of the expired refresh tokens has failed.", ex);

                return InternalServerError();
            }

            _logger.LogInformation("Expired refresh tokens have been successfully deleted.");

            return NoContent();
        }

        // TODO - need to move to the web application (USE AuthenticationSchemeProvider)
        /// <summary>TODO REMOVE - Retrieves the names of the supported external login providers.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("external/providers")]
        public async Task<IActionResult> ProvidersAsync()
        {
            IEnumerable<AuthenticationScheme> schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            IList<string> providerNames = schemes.Select(s => s.DisplayName).ToList();

            return Ok(providerNames);
        }

        // TODO - need to move to the web application (USE AuthenticationSchemeProvider)
        /// <summary>TODO REMOVE - Initiates the external login authentication process and redirects the user to the provider auth page.</summary>
        /// <param name="provider">Provider name or Id which uniquely identifies social login for which access token should be issued.</param>
        /// <param name="returnUrl">The return url.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("external/initiate/{provider}")]
        public async Task<IActionResult> InitiateAuthentictionAsync([FromRoute] string provider, [FromQuery] string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable<AuthenticationScheme> schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            AuthenticationScheme authScheme = schemes.Where(el => el.Name.ToUpper().Equals(provider.ToUpper())).FirstOrDefault();

            if (authScheme == null)
            {
                return BadRequest("Not supported external login provider.");
            }

            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(authScheme.Name, returnUrl);

            return Challenge(properties, authScheme.Name);
        }

        /// <summary>Authenticates the user via the external login request.</summary>
        /// <param name="authenticateModel">The authenticate external login model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Identity.External")]
        [Route("external")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticateExternalRequestApiModel authenticateModel)
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

            string clientAuthResult = await _authManager.AuthenticateClientAsync(clientId, authenticateModel.ClientSecret);
            if (!string.IsNullOrEmpty(clientAuthResult))
            {
                return Unauthorized(clientAuthResult);
            }

            ExternalLoginInfo loginInfo = await _signInManager.GetExternalLoginInfoAsync();     
            if (loginInfo == null)
            {
                return BadRequest("External login information is missing.");
            }

            // Sign in the user with this external login provider if the user already has a *confirmed* external login.
            IUser user = await _userManager.FindUserByLoginAsync(loginInfo, loginConfirmed: true);
            if (user != null)
            {
                _logger.LogInformation($"Trying to sign in user {user.Email} with the existing external login provider.");

                SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);

                return await AuthenticateAsync(signInResult, user, clientId, ExternalAuthStep.FoundExistingExternalLogin, loginInfo.LoginProvider);
            }

            string userEmail = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest($"Email scope access is required to add {loginInfo.ProviderDisplayName} provider.");
            }

            user = await _userManager.FindByEmailAsync(userEmail);

            if (user != null)
            {
                _logger.LogInformation($"There is user account registered with {userEmail} email.");

                if (user.IsDeleted || !user.IsApproved)
                {
                    _logger.LogInformation("User is deleted or not approved.");

                    return Ok(new AuthenticateResponseApiModel { ExternalAuthStep = ExternalAuthStep.UserNotAllowed });
                }

                _logger.LogInformation($"Email {userEmail} is {(user.EmailConfirmed ? "confirmed" : "not confirmed")}.");

                IdentityResult createLoginResult;

                if (!user.EmailConfirmed)
                {
                    // Generate token for external login confirmation
                    string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Add the external provider (confirmed = false)
                    createLoginResult = await _userManager.AddOrUpdateLoginAsync(user, loginInfo, token);

                    if (!createLoginResult.Succeeded) return GetErrorResult(createLoginResult);

                    // Send email
                    UriTemplate template = new UriTemplate(authenticateModel.ConfirmationUrl);
                    string callbackUrl = template.Resolve(new Dictionary<string, object>
                    {
                        { "userId", user.Id.ToString() },
                        { "token", token.Base64ForUrlEncode() }
                    });

                    _logger.LogInformation($"Sending email confirmation token to confirm association of {loginInfo.ProviderDisplayName} external login account.");

                    await _emailClientSender.SendConfirmExternalAccountEmailAsync(user.Email, callbackUrl, loginInfo.ProviderDisplayName);

                    return Ok(new AuthenticateResponseApiModel { ExternalAuthStep = ExternalAuthStep.PendingExternalLoginCreation, VerificationStep = VerificationStep.Email });
                }

                // Add the external provider (confirmed = true)
                createLoginResult = await _userManager.AddLoginAsync(user, loginInfo);

                if (!createLoginResult.Succeeded) return GetErrorResult(createLoginResult);

                _logger.LogInformation($"Trying to sign in user {user.Email} with new external login provider.");

                SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                return await AuthenticateAsync(signInResult, user, clientId, ExternalAuthStep.AddedNewExternalLogin, loginInfo.LoginProvider);
            }

            _logger.LogInformation($"There is no user account registered with {userEmail} email.");
            _logger.LogInformation($"A new user account must be created or external login must be associated with different email address.");

            return Ok(new AuthenticateResponseApiModel { ExternalAuthStep = ExternalAuthStep.UserNotFound });
        }

        /// <summary>Authenticates the user using the two factor authentication code.</summary>
        /// <param name="authenticateModel">The authenticate model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Route("two-factor")]
        public async Task<IActionResult> AuthenticateUserAsync(AuthenticateTwoFactorRequestApiModel authenticateModel)
        {
            // TODO - need to remove clientId from model

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
            if (authenticateModel.UseRecoveryCode)
            {
                signInResult = await _signInManager.TwoFactorRecoveryCodeSignInAsync(authenticateModel.Code);
            }
            else
            {
                //Note: isPersistent, rememberClient: false - no need to store browser cookies in Web API.
                signInResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticateModel.Code, false, false);
            }

            return await AuthenticateAsync(signInResult, user, clientId);
        }

        /// <summary>Generates or retrieves authenticator key for the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("two-factor/{userId:guid}/authenticator")]
        public async Task<IActionResult> GetUserAuthenticatorKeyAsync([FromRoute] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            bool hasPermissions = IsCurrentUser(userId) || User.IsInRole(RoleHelper.Admin);
            if (!hasPermissions)
            {
                return Forbid();
            }

            IUser user = await _userManager.FindUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

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

        /// <summary>Verifies the authenticator code for the user. If successful, two factor authentication will be enabled.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="code">The authenticator code.</param>
        /// <returns>
        ///   Ten two factor recovery codes.
        /// </returns>
        [HttpPost]
        [Authorize]
        [Route("two-factor/{userId:guid}/authenticator")]
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

            bool hasPermissions = IsCurrentUser(userId) || User.IsInRole(RoleHelper.Admin);
            if (!hasPermissions)
            {
                return Forbid();
            }

            IUser user = await _userManager.FindUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

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
        /// Generates two factor recovery codes for the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="number">The number.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("two-factor/{userId:guid}/recovery-codes")]
        public async Task<IActionResult> GenerateNewRecoveryCodesAsync([FromRoute] Guid userId, [FromQuery] int number)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            bool hasPermissions = IsCurrentUser(userId) || User.IsInRole(RoleHelper.Admin);
            if (!hasPermissions)
            {
                return Forbid();
            }

            IUser user = await _userManager.FindUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

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

        #region Helpers

        private static string GenerateAuthenticatorUri(string email, string authenticatorKey)
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

        private async Task<IActionResult> AuthenticateAsync
        (
           SignInResult signInResult,
           IUser user,
           Guid clientId,
           ExternalAuthStep externalLoginStep = ExternalAuthStep.None,
           string externalLoginProvider = null
        )
        {
            if (signInResult == null)
                throw new ArgumentNullException(nameof(signInResult));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (GuidHelper.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            AuthenticateResponseApiModel authResponse = new AuthenticateResponseApiModel
            {
                UserId = user.Id,
                VerificationStep = VerificationStep.None,
                ExternalAuthStep = externalLoginStep
            };

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    return Unauthorized($"User [{user.UserName}] has been locked out.");
                }
                if (signInResult.IsNotAllowed)
                {
                    if (!user.EmailConfirmed) authResponse.VerificationStep = VerificationStep.Email;
                    else if (!user.PhoneNumberConfirmed) authResponse.VerificationStep = VerificationStep.MobilePhone;
                    else throw new Exception("Invalid use case.");

                    return Ok(authResponse);
                }
                if (signInResult.RequiresTwoFactor)
                {
                    authResponse.VerificationStep = VerificationStep.TwoFactor;

                    return Ok(authResponse);
                }

                return Unauthorized($"Failed to log in [{user.UserName}].");
            }

            _logger.LogInformation($"User [{user.UserName}] has logged in the system.");

            IJwtAuthResult jwtResult = await _authManager.GenerateTokensAsync(user.Id, clientId, externalLoginProvider);

            authResponse.Roles = jwtResult.Roles.ToArray();
            authResponse.AccessToken = jwtResult.AccessToken;
            authResponse.RefreshToken = jwtResult.RefreshToken;

            if (signInResult.Succeeded)
            {
                // Need to delete the "identity" cookie for the authorized user - created by SignInManager 
                Response.Cookies.Delete(".AspNetCore.Identity.Application");
                Response.Headers.Remove("Set-Cookie");
            }

            return Ok(authResponse);
        }

        #endregion
    }
}