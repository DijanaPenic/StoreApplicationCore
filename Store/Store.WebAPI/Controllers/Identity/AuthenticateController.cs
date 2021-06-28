using System;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using X.PagedList;
using Resta.UriTemplates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Common.Extensions;
using Store.Common.Parameters;
using Store.Services.Identity;
using Store.Service.Common.Services.Identity;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Infrastructure.Authorization.Attributes;
using Store.WebAPI.Infrastructure.Authorization.Extensions;
using Store.Messaging.Services.Common;
using Store.Model.Common.Models.Identity;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/authenticate")]
    public class AuthenticateController : ApplicationControllerBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationAuthManager _authManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEmailService _emailService;

        public AuthenticateController
        (
            ApplicationUserManager userManager,
            ApplicationAuthManager authManager,
            ApplicationSignInManager signInManager,
            IAuthorizationService authorizationService,
            ILogger<AuthenticateController> logger,
            IEmailService emailService,
            IQueryUtilityFacade queryUtilityFacade
        ) : base(queryUtilityFacade)
        {
            _userManager = userManager;
            _authManager = authManager;
            _signInManager = signInManager;
            _authorizationService = authorizationService;
            _logger = logger;
            _emailService = emailService;
        }

        /// <summary>Retrieves the authentication info for the currently logged in user.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("info")]
        [Produces("application/json")]
        public async Task<IActionResult> AuthenticateInfoAsync()
        {
            bool isUserAuthenticated = User.Identity is {IsAuthenticated: true};

            AuthenticateInfoGetApiModel authInfoModel = new AuthenticateInfoGetApiModel
            {
                IsAuthenticated = isUserAuthenticated,
                Username = isUserAuthenticated ? User.Identity.Name : string.Empty,
                ExternalLoginProvider =
                    User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationMethod)?.Value,
                DisplaySetPassword = isUserAuthenticated &&
                                     !(await _userManager.HasPasswordAsync((await _userManager.GetUserAsync(User))))
            };

            return Ok(authInfoModel);
        }

        /// <summary>Attempts to authenticate user at the end of the account verification process (account verification: phone number and/or email verification).</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("account-verification")]
        [Produces("application/json")]
        public async Task<IActionResult> AuthenticateAsync()
        {
            // Retrieve user account information from cookie
            AccountVerificationInfo accountVerificationInfo = await _signInManager.GetAccountVerificationInfoAsync();
            if (accountVerificationInfo == null)
            {
                return BadRequest("Account Verification information not found.");
            }

            // Verify user information
            if (string.IsNullOrEmpty(accountVerificationInfo.UserId))
            {
                return BadRequest("User Id cannot be empty.");
            }

            IUser user = await _userManager.FindByIdAsync(accountVerificationInfo.UserId);
            if (user == null)
            {
                return NotFound("User Id not found.");
            }

            Guid clientId = Guid.Parse(accountVerificationInfo.ClientId);
            SignInResult signInResult = await _signInManager.AccountVerificationSignInAsync(clientId);

            return await AuthenticateAsync(signInResult, user, clientId);
        }

        /// <summary>Attempts to authenticate user using the specified username and password combination.</summary>
        /// <param name="authenticateModel">The authenticate model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [ClientAuthorization]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> AuthenticateAsync(
            [FromBody] AuthenticatePasswordPostApiModel authenticateModel)
        {
            // Check user's status
            IUser user = await _userManager.FindByNameAsync(authenticateModel.UserName);
            if (user == null)
            {
                return Unauthorized($"Failed to log in - invalid username and/or password.");
            }

            if (!user.IsApproved)
            {
                return Unauthorized($"User [{user.UserName}] is not approved.");
            }

            Guid clientId = GetCurrentUserClientId();

            // Attempt to sign in
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(clientId, user,
                authenticateModel.Password, lockoutOnFailure: true);

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
        [Produces("application/json")]
        public async Task<IActionResult> RenewTokensAsync([FromRoute] string refreshToken)
        {
            try
            {
                // Generate new tokens
                string accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                IJwtAuthResult jwtResult = await _authManager.RenewTokensAsync(refreshToken.Base64Decode(), accessToken, GetCurrentUserClientId());

                _logger.LogInformation("New access and refresh tokens are generated for the user.");

                RenewTokenGetApiModel authenticationResponse = new RenewTokenGetApiModel
                {
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken
                };

                return Ok(authenticationResponse);
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Deletes refresh tokens that have expired.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpDelete]
        [Route("expired-refresh-tokens")]
        [SectionAuthorization(SectionType.User, AccessType.Full)]
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

        // TODO - need to move to the web application (USE AuthenticationSchemeProvider). Current test: https://store.com:5000/api/authenticate/external/initiate/Google?return_url=https%3A%2F%2Fstore.com%3A5000%2F
        /// <summary>TODO REMOVE - Initiates the external login authentication process and redirects the user to the provider auth page.</summary>
        /// <param name="provider">Provider name or Id which uniquely identifies social login for which access token should be issued.</param>
        /// <param name="returnUrl">The return url.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("external/initiate/{provider}")]
        public async Task<IActionResult> InitiateAuthenticationAsync([FromRoute] string provider,
            [FromQuery] string returnUrl)
        {
            IEnumerable<AuthenticationScheme> schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            AuthenticationScheme authScheme = schemes.FirstOrDefault(el => el.Name.ToUpper().Equals(provider.ToUpper()));

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
        [ClientAuthorization]
        [Authorize(AuthenticationSchemes = "Identity.External")]
        [Route("external")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateExternalPostApiModel authenticateModel)
        {
            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return BadRequest("External login information is missing.");
            }

            // Retrieve client_id
            Guid clientId = GetCurrentUserClientId();

            // Sign in the user with this external login provider if the user already has a *confirmed* external login.
            IUser user = await _userManager.FindUserByLoginAsync(externalLoginInfo, loginConfirmed: true);
            if (user != null)
            {
                _logger.LogInformation($"Trying to sign in user {user.Email} with the existing external login provider.");

                SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync
                (
                    clientId, 
                    externalLoginInfo.LoginProvider, 
                    externalLoginInfo.ProviderKey, 
                    bypassTwoFactor: true
                );

                return await AuthenticateAsync(signInResult, user, clientId, ExternalAuthStep.FoundExistingExternalLogin, externalLoginInfo.LoginProvider);
            }

            string userEmail = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest($"Email scope access is required to add {externalLoginInfo.ProviderDisplayName} provider.");
            }

            user = await _userManager.FindByEmailAsync(userEmail);

            if (user != null)
            {
                _logger.LogInformation($"There is user account registered with {userEmail} email.");

                if (!user.IsApproved)
                {
                    _logger.LogInformation("User is deleted or not approved.");

                    return Ok(new AuthenticateGetApiModel {ExternalAuthStep = ExternalAuthStep.UserNotAllowed});
                }

                _logger.LogInformation($"Email {userEmail} is {(user.EmailConfirmed ? "confirmed" : "not confirmed")}.");

                IdentityResult createLoginResult;

                if (!user.EmailConfirmed)
                {
                    // Generate token for external login confirmation
                    string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Add the external provider (confirmed = false)
                    createLoginResult = await _userManager.AddOrUpdateLoginAsync(user, externalLoginInfo, token);

                    if (!createLoginResult.Succeeded) return BadRequest(createLoginResult.Errors);

                    // Send email
                    UriTemplate template = new UriTemplate(authenticateModel.ConfirmationUrl);
                    string callbackUrl = template.Resolve(new Dictionary<string, object>
                    {
                        {"userId", user.Id.ToString()},
                        {"token", token.Base64Encode()}
                    });

                    _logger.LogInformation($"Sending email confirmation token to confirm association of {externalLoginInfo.ProviderDisplayName} external login account.");

                    await _emailService.SendConfirmExternalAccountAsync(clientId, user.Email, callbackUrl, externalLoginInfo.ProviderDisplayName);

                    return Ok(new AuthenticateGetApiModel
                    {
                        ExternalAuthStep = ExternalAuthStep.PendingExternalLoginCreation,
                        VerificationStep = VerificationStep.Email
                    });
                }

                // Add the external provider (confirmed = true)
                createLoginResult = await _userManager.AddLoginAsync(user, externalLoginInfo);

                if (!createLoginResult.Succeeded) return BadRequest(createLoginResult.Errors);

                _logger.LogInformation($"Trying to sign in user {user.Email} with new external login provider.");

                SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync
                (
                    clientId, 
                    externalLoginInfo.LoginProvider, 
                    externalLoginInfo.ProviderKey, 
                    bypassTwoFactor: true
                );

                return await AuthenticateAsync(signInResult, user, clientId, ExternalAuthStep.AddedNewExternalLogin, externalLoginInfo.LoginProvider);
            }

            _logger.LogInformation($"There is no user account registered with {userEmail} email.");
            _logger.LogInformation($"A new user account must be created or external login must be associated with different email address.");

            return Ok(new AuthenticateGetApiModel {ExternalAuthStep = ExternalAuthStep.UserNotFound});
        }

        /// <summary>Authenticates the user using the two factor authentication code.</summary>
        /// <param name="authenticateModel">The authenticate model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("two-factor")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateTwoFactorPostApiModel authenticateModel)
        {
            TwoFactorAuthenticationInfo twoFactorInfo = await _signInManager.GetTwoFactorInfoAsync();
            if (twoFactorInfo == null)
            {
                return BadRequest("Two Factor authentication info not found.");
            }

            IUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return BadRequest("Two-factor authentication action is not supported for the user.");
            }

            Guid clientId = Guid.Parse(twoFactorInfo.ClientId);
            SignInResult signInResult;

            if (authenticateModel.UseRecoveryCode)
            {
                signInResult = await _signInManager.TwoFactorRecoveryCodeSignInAsync(authenticateModel.Code);
            }
            else
            {
                //Note: rememberClient: false - don't want to suppress future two-factor auth requests.
                signInResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(clientId, authenticateModel.Code, rememberClient: false);
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

                _logger.LogInformation("Two factor authentication is enabled for the user.");
            }
            else
            {
                return BadRequest("Verification Code is invalid.");
            }

            if (await _userManager.CountRecoveryCodesAsync(user) != 0) return NoContent();
            
            IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            _logger.LogInformation("Ten two factor recovery codes are generated for the user.");

            TwoFactorRecoveryResponseApiModel response = new TwoFactorRecoveryResponseApiModel
            {
                RecoveryCodes = recoveryCodes.ToArray()
            };

            return Ok(response);

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

            TwoFactorRecoveryResponseApiModel response = new TwoFactorRecoveryResponseApiModel
            {
                RecoveryCodes = recoveryCodes.ToArray()
            };

            return Ok(response);
        }

        #region Helpers

        private static string GenerateAuthenticatorUri(string email, string authenticatorKey)
        {
            const string authenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

            return string.Format
            (
                authenticatorUriFormat,
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

            AuthenticateGetApiModel authResponse = new AuthenticateGetApiModel
            {
                UserId = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
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

                if (!signInResult.RequiresTwoFactor) return Unauthorized($"Failed to log in [{user.UserName}].");

                authResponse.VerificationStep = VerificationStep.TwoFactor;

                return Ok(authResponse);
            }

            _logger.LogInformation($"User [{user.UserName}] has logged in the system.");

            IJwtAuthResult jwtResult = await _authManager.GenerateTokensAsync(user.Id, clientId, externalLoginProvider);

            authResponse.Roles = jwtResult.Roles.ToArray();
            authResponse.AccessToken = jwtResult.AccessToken;
            authResponse.RefreshToken = jwtResult.RefreshToken;

            return Ok(authResponse);
        }

        #endregion
    }
}