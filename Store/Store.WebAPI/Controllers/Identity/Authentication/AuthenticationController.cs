using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Resta.UriTemplates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Common.Extensions;
using Store.Common.Parameters;
using Store.Services.Identity;
using Store.Service.Common.Services.Identity;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Infrastructure.Authorization.Attributes;
using Store.Messaging.Services.Common;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ApplicationControllerBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationAuthManager _authManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IEmailService _emailService;

        public AuthenticationController
        (
            ApplicationUserManager userManager,
            ApplicationAuthManager authManager,
            ApplicationSignInManager signInManager,
            ILogger<AuthenticationController> logger,
            IEmailService emailService,
            IQueryUtilityFacade queryUtilityFacade
        ) : base(queryUtilityFacade)
        {
            _userManager = userManager;
            _authManager = authManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService;
        }

        /// <summary>Retrieves the authentication information for the currently logged in user.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Produces("application/json")]
        public async Task<IActionResult> AuthenticateInfoAsync()
        {
            bool isUserAuthenticated = User.Identity is {IsAuthenticated: true};

            AuthenticateInfoGetApiModel authInfoModel = new()
            {
                IsAuthenticated = isUserAuthenticated,
                Username = isUserAuthenticated ? User.Identity.Name : string.Empty,
                ExternalLoginProvider = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationMethod)?.Value,
                DisplaySetPassword = isUserAuthenticated && !(await _userManager.HasPasswordAsync((await _userManager.GetUserAsync(User))))
            };

            return Ok(authInfoModel);
        }
        
        /// <summary>Renews the authentication tokens (refresh and access tokens).</summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPut]
        [Authorize]
        [Produces("application/json")]
        public async Task<IActionResult> RenewTokensAsync([FromQuery] string refreshToken)
        {
            if (!refreshToken.IsBase64String())
            {
                return BadRequest("Provided token is not in valid format.");
            }
            
            try
            {
                // Generate new tokens
                string accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                IJwtAuthResult jwtResult = await _authManager.RenewTokensAsync(refreshToken.Base64Decode(), accessToken, GetCurrentUserClientId());

                _logger.LogInformation("New access and refresh tokens are generated for the user.");

                RenewTokenGetApiModel authenticationResponse = new()
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
        
        /// <summary>Attempts to authenticate user using the provided username and password combination.</summary>
        /// <param name="authenticateModel">The authenticate model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [ClientAuthorization]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticatePasswordPostApiModel authenticateModel)
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

            // Retrieve client_id
            Guid clientId = GetCurrentUserClientId();

            // Attempt to sign in
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(clientId, user,
                authenticateModel.Password, lockoutOnFailure: true);

            return await AuthenticateAsync(signInResult, user);
        }

        // TODO - need to move to the web application (USE AuthenticationSchemeProvider)
        /// <summary>REMOVE - Retrieves the names of the supported external login providers.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("external/providers")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ProvidersAsync()
        {
            IEnumerable<AuthenticationScheme> schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            IList<string> providerNames = schemes.Select(s => s.DisplayName).ToList();

            return Ok(providerNames);
        }

        // TODO - need to move to the web application (USE AuthenticationSchemeProvider). Current test: https://store.com:5000/api/authenticate/external/initiate/Google?return_url=https%3A%2F%2Fstore.com%3A5000%2F
        /// <summary>REMOVE - Initiates the external login authentication process and redirects the user to the provider auth page.</summary>
        /// <param name="provider">Provider name or Id which uniquely identifies social login for which access token should be issued.</param>
        /// <param name="returnUrl">The return url.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("external/initiate/{provider}")]
        [ApiExplorerSettings(IgnoreApi = true)]
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

        /// <summary>Attempts to authenticate user via the external login request.</summary>
        /// <param name="authenticateModel">The authenticate external login model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [ClientAuthorization]
        //[Authorize(AuthenticationSchemes = "Identity.External")] -> Cannot combine with the ClientAuthorization scheme
        [Route("external-login")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateExternalPostApiModel authenticateModel)
        {
            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return Unauthorized("External authentication has failed."); 
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

                ExternalAuthResult externalAuthResult = new() { Step = ExternalAuthStep.FoundExistingExternalLogin, LoginProvider = externalLoginInfo.LoginProvider};

                return await AuthenticateAsync(signInResult, user, externalAuthResult);
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
                    UriTemplate template = new(authenticateModel.ConfirmationUrl);
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

                ExternalAuthResult externalAuthResult = new() { Step = ExternalAuthStep.AddedNewExternalLogin, LoginProvider = externalLoginInfo.LoginProvider};
                
                return await AuthenticateAsync(signInResult, user, externalAuthResult);
            }

            _logger.LogInformation($"There is no user account registered with {userEmail} email.");
            _logger.LogInformation($"A new user account must be created or external login must be associated with different email address.");

            return Ok(new AuthenticateGetApiModel {ExternalAuthStep = ExternalAuthStep.UserNotFound});
        }

        /// <summary>Attempts to authenticate user using the two-factor authentication code.</summary>
        /// <param name="authenticateModel">The authenticate model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Identity.TwoFactorUserId")]
        [Route("two-factor")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateTwoFactorPostApiModel authenticateModel)
        {
            TwoFactorAuthenticationInfo twoFactorInfo = await _signInManager.GetTwoFactorInfoAsync();
            if (twoFactorInfo == null)
            {
                return Unauthorized("Two Factor authentication has failed.");
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

            return await AuthenticateAsync(signInResult, user);
        }

        /// <summary>Attempts to authenticate user at the end of the account verification process (account verification: phone number and/or email verification).</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Identity.AccountVerification")]
        [Route("account-verification")]
        [Produces("application/json")]
        public async Task<IActionResult> AuthenticateAsync()
        {
            // Retrieve user account information from cookie
            AccountVerificationInfo accountVerificationInfo = await _signInManager.GetAccountVerificationInfoAsync();
            if (accountVerificationInfo == null || string.IsNullOrEmpty(accountVerificationInfo.UserId))
            {
                return Unauthorized("Account authentication has failed.");
            }

            IUser user = await _userManager.FindByIdAsync(accountVerificationInfo.UserId);
            if (user == null)
            {
                return NotFound("User Id not found.");
            }

            Guid clientId = Guid.Parse(accountVerificationInfo.ClientId);
            SignInResult signInResult = await _signInManager.AccountVerificationSignInAsync(clientId);

            return await AuthenticateAsync(signInResult, user);
        }
        
        /// <summary>Deletes refresh tokens that have expired.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpDelete]
        [Route("refresh-tokens")]
        [SectionAuthorization(SectionType.User, AccessType.Full)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteRefreshTokensAsync()
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

        #region Helpers
        
        private async Task<IActionResult> AuthenticateAsync(SignInResult signInResult, IUser user, ExternalAuthResult externalAuthResult = null)
        {
            if (signInResult == null)
                throw new ArgumentNullException(nameof(signInResult));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            AuthenticateGetApiModel authResponse = new()
            {
                UserId = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                VerificationStep = VerificationStep.None,
                ExternalAuthStep = externalAuthResult?.Step ?? ExternalAuthStep.None
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
            
            IJwtAuthResult jwtResult = await _authManager.GenerateTokensAsync(user.Id, GetCurrentUserClientId(), externalAuthResult?.LoginProvider);

            authResponse.Roles = jwtResult.Roles.ToArray();
            authResponse.AccessToken = jwtResult.AccessToken;
            authResponse.RefreshToken = jwtResult.RefreshToken;

            return Ok(authResponse);
        }

        private class ExternalAuthResult
        {
            public ExternalAuthStep Step { get; init; }

            public string LoginProvider { get; init; }
        }

        #endregion
    }
}