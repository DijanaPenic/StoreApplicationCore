﻿using System;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Store.WebAPI.Models;
using Store.WebAPI.Identity;
using Store.WebAPI.Infrastructure;
using Store.Common.Helpers;
using Store.Common.Helpers.Identity;
using Store.Models.Api.Identity;
using Store.Model.Common.Models.Identity;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("authenticate")]
    public class AuthenticateController : IdentityControllerBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationAuthManager _authManager;
        private readonly SignInManager<IUser> _signInManager;

        public AuthenticateController
        (
            ApplicationUserManager userManager,
            ApplicationAuthManager authManager,
            SignInManager<IUser> signInManager,
            ILogger<AuthenticateController> logger
        )
        : base(userManager)
        {
            _userManager = userManager;
            _authManager = authManager;
            _signInManager = signInManager;
            _logger = logger;
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
        /// <param name="renewTokenModel">The renew token model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("renew-tokens")]
        public async Task<IActionResult> RenewTokensAsync([FromBody] RenewTokenRequestApiModel renewTokenModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify client information
            if (!Guid.TryParse(renewTokenModel.ClientId, out Guid clientId) || GuidHelper.IsNullOrEmpty(clientId))
            {
                return BadRequest($"Client '{clientId}' format is invalid.");
            }

            string clientAuthResult = await _authManager.AuthenticateClientAsync(clientId, renewTokenModel.ClientSecret);
            if (!string.IsNullOrEmpty(clientAuthResult))
            {
                return Unauthorized(clientAuthResult);
            }

            try
            {
                // Generate new tokens
                string accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                JwtAuthResult jwtResult = await _authManager.RefreshTokensAsync(renewTokenModel.RefreshToken, accessToken, clientId);

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

        /// <summary>Retrieves the names of the supported external login providers.</summary>
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

        /// <summary>Initiates the external login authentication process.</summary>
        /// <param name="provider">Provider name or Id which uniquely identifies social login for which access token should be issued.</param>
        /// <param name="returnUrl">The return url.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
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

            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest("External login information is missing.");
            }

            // Sign in the user with this external login provider if the user already has an external login.
            IUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user != null)
            {
                _logger.LogInformation($"Trying to sign in user {user.Email} with the existing external login provider.");

                SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

                return await AuthenticateAsync(signInResult, user, clientId, ExternalLoginStatus.ExistingExternalLoginSuccess, info.LoginProvider);
            }

            string userEmail = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest($"Email scope access is required to add {info.ProviderDisplayName} provider.");
            }

            user = await _userManager.FindByEmailAsync(userEmail);

            if (user != null)
            {
                _logger.LogInformation($"There is user account registered with {userEmail} email.");

                if (user.IsDeleted || !user.IsApproved)
                {
                    _logger.LogInformation("User is deleted or not approved.");

                    return Ok(new AuthenticateResponseApiModel { ExternalLoginStatus = ExternalLoginStatus.UserNotAllowed });
                }

                _logger.LogInformation($"Email {userEmail} is {(user.EmailConfirmed ? "confirmed" : "not confirmed")}.");

                if (!user.EmailConfirmed)
                {
                    string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    string callbackUrl = Url.Action
                    (
                        "ConfirmExternalProvider",
                        "ExternalLogin",
                        values: new
                        {
                            id = user.Id,
                            clientId,
                            token,
                            loginProvider = info.LoginProvider,
                            loginProviderDisplayName = info.LoginProvider,
                            providerKey = info.ProviderKey
                        },
                        protocol: Request.Scheme)
                    ;

                    _logger.LogInformation($"Sending email confirmation token to confirm association of {info.ProviderDisplayName} external login account.");

                    // TODO - missing email implementation
                    //await _emailSender.SendEmailAsync
                    //(
                    //    user.Email, 
                    //    $"Confirm {info.ProviderDisplayName} external login",
                    //    $"Please confirm association of your {info.ProviderDisplayName} account by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>."
                    //);

                    return Ok(new AuthenticateResponseApiModel { ExternalLoginStatus = ExternalLoginStatus.PendingEmailConfirmation });
                }

                // Add the external provider
                await _userManager.AddLoginAsync(user, info);

                _logger.LogInformation($"Trying to sign in user {user.Email} with new external login provider.");

                SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                return await AuthenticateAsync(signInResult, user, clientId, ExternalLoginStatus.NewExternalLoginAddedSuccess, info.LoginProvider);
            }

            _logger.LogInformation($"There is no user account registered with {userEmail} email.");
            _logger.LogInformation($"A new user account must be created or external login must be associated with different email address.");

            return Ok(new AuthenticateResponseApiModel { ExternalLoginStatus = ExternalLoginStatus.UserAccountNotFound });
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

        /// <summary>Generates or retrieves authenticator key for the currently logged in user.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("two-factor/authenticator")]
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
        [Route("two-factor/authenticator")]
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
        [Route("two-factor/recovery-codes")]
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

        #region Helpers

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

        private async Task<IActionResult> AuthenticateAsync
        (
           SignInResult signInResult,
           IUser user,
           Guid clientId,
           ExternalLoginStatus externalLoginStatus = ExternalLoginStatus.None,
           string provider = null
        )
        {
            if (signInResult == null)
                throw new ArgumentNullException(nameof(signInResult));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (GuidHelper.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            AuthenticateResponseApiModel authenticationResponse = new AuthenticateResponseApiModel
            {
                UserId = user.Id,
                RequiresTwoFactor = signInResult.RequiresTwoFactor,
                ExternalLoginStatus = externalLoginStatus
            };

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    return Unauthorized($"User [{user.UserName}] has been locked out.");
                }
                if (signInResult.IsNotAllowed)
                {
                    return Unauthorized($"User [{user.UserName}] is not allowed to log in.");
                }
                if (signInResult.RequiresTwoFactor)
                {
                    return Ok(authenticationResponse);
                }

                return Unauthorized($"Failed to log in [{user.UserName}].");
            }

            _logger.LogInformation($"User [{user.UserName}] has logged in the system.");

            JwtAuthResult jwtResult = await _authManager.GenerateTokensAsync(user.Id, clientId, provider);

            authenticationResponse.Roles = jwtResult.Roles.ToArray();
            authenticationResponse.AccessToken = jwtResult.AccessToken;
            authenticationResponse.RefreshToken = jwtResult.RefreshToken;

            if (signInResult.Succeeded)
            {
                // Need to delete the "identity" cookie for the authorized user - created by SignInManager 
                Response.Cookies.Delete(".AspNetCore.Identity.Application");
                Response.Headers.Remove("Set-Cookie");
            }

            return Ok(authenticationResponse);
        }

        #endregion
    }
}