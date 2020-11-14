using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

using Store.Common.Helpers;
using Store.WebAPI.Identity;
using Store.Models.Identity;
using Store.Models.Api.Identity;
using Store.Model.Common.Models.Identity;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

// Blog post: https://chsakell.com/2019/07/28/asp-net-core-identity-series-external-provider-authentication-registration-strategy/
// TODO - add information about authentication rules
namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("external-login")]
    public class ExternalLoginController : IdentityControllerBase
    {
        private readonly ApplicationUserManager _userManager;
        private readonly SignInManager<IUser> _signInManager;
        private readonly ApplicationAuthManager _authManager;
        private readonly ILogger _logger;       

        public ExternalLoginController
        (
            ApplicationUserManager userManager,
            SignInManager<IUser> signInManager,
            ApplicationAuthManager authManager,
            ILogger<AccountController> logger
        ) 
        : base (authManager, logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authManager = authManager;
            _logger = logger;
        } 

        /// <summary>Retrieves names of the supported external login providers.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("providers")]
        public async Task<IActionResult> ProvidersAsync()
        {
            IEnumerable<AuthenticationScheme> schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            IList<string> providerNames = schemes.Select(s => s.DisplayName).ToList();

            return Ok(providerNames);
        }

        /// <summary>Initiates the external login authentication process.</summary>
        /// <param name="provider">The provider.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("authentiate")] // TODO - need to use model from body instead of query parameters + POST
        public IActionResult Authenticate([FromQuery] string provider, [FromQuery] Guid clientId, [FromQuery] string clientSecret = null)
        {
            string redirectUrl = Url.Action("AuthenticateCallback", "ExternalLogin", new { clientId, clientSecret });        // Url.Action is not working if "CallbackAsync" value is used
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }

        /// <summary>Authenticates the user via his external login request.</summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("authentiate-callback")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> AuthenticateCallbackAsync([FromQuery] Guid clientId, [FromQuery] string clientSecret = null)
        {
            if (GuidHelper.IsNullOrEmpty(clientId))
            {
                return BadRequest("Client Id is required.");
            }

            string clientAuthResult = await _authManager.AuthenticateClientAsync(clientId, clientSecret);
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

                return await AuthenticateAsync(signInResult, user, clientId, ExternalLoginStatus.ExistingExternalLoginSuccess);
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
                return await AuthenticateAsync(signInResult, user, clientId, ExternalLoginStatus.NewExternalLoginAddedSuccess);
            }

            _logger.LogInformation($"There is no user account registered with {userEmail} email.");
            _logger.LogInformation($"A new user account must be created or external login must be associated with different email address.");

            return Ok(new AuthenticateResponseApiModel { ExternalLoginStatus = ExternalLoginStatus.UserAccountNotFound });
        }

        /// <summary>Confirms the external provider.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="token">The token.</param>
        /// <param name="loginProvider">The external login provider.</param>
        /// <param name="loginProviderDisplayName">Display name of the external login provider.</param>
        /// <param name="providerKey">The provider key.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet] // Must be GET because it will be called via email link
        [AllowAnonymous]
        [Route("users/{userId:guid}/confirm-external-provider")]
        public async Task<IActionResult> ConfirmExternalProviderAsync([FromRoute] Guid userId, [FromQuery]Guid clientId, [FromQuery]string token, 
        [FromQuery]string loginProvider, [FromQuery]string loginProviderDisplayName, [FromQuery]string providerKey)
        {
            if (GuidHelper.IsNullOrEmpty(userId))
            {
                return BadRequest("User Id is missing.");
            }
            if (GuidHelper.IsNullOrEmpty(clientId))
            {
                return BadRequest("Client Id is missing.");
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Token is required.");
            }
            if (string.IsNullOrWhiteSpace(loginProvider))
            {
                return BadRequest("Login provider is required.");
            }
            if (string.IsNullOrWhiteSpace(loginProviderDisplayName))
            {
                return BadRequest("Provider display name is required.");
            }
            if (string.IsNullOrWhiteSpace(providerKey))
            {
                return BadRequest("Provider key is required.");
            }

            IUser user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                NotFound();
            }

            // External provider is authenticated source so we can confirm the email
            IdentityResult emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!emailConfirmationResult.Succeeded)
                return InternalServerError();

            IdentityResult newExternalLoginResult = await _userManager.AddLoginAsync(user, new ExternalLoginInfo(null, loginProvider, providerKey, loginProviderDisplayName));
            if (!newExternalLoginResult.Succeeded)
                return InternalServerError();

            SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent: false, bypassTwoFactor: true);
            return await AuthenticateAsync(signInResult, user, clientId, ExternalLoginStatus.NewExternalLoginAddedSuccess);
        }

        [HttpPost]
        [Route("register")]
        // TODO - authentication 
        public async Task<IActionResult> RegisterAsync([FromBody] ExternalLoginRegisterRequestApiModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify client information
            if (!Guid.TryParse(registerModel.ClientId, out Guid clientId) || GuidHelper.IsNullOrEmpty(clientId))
            {
                return BadRequest($"Client '{clientId}' format is invalid.");
            }

            // Create a new account
            if (!registerModel.AssociateExistingAccount)
            {
                if(string.IsNullOrWhiteSpace(registerModel.Username) || string.IsNullOrWhiteSpace(registerModel.ExternalLoginEmail))
                {
                    return BadRequest("Both username and external login email are required.");
                }

                IUser newUser = new User 
                { 
                    UserName = registerModel.Username, 
                    Email = registerModel.ExternalLoginEmail 
                };

                _logger.LogInformation($"Creating a new user {registerModel.ExternalLoginEmail}.");

                // Create a new user
                IdentityResult createUserResult = await _userManager.CreateAsync(newUser);
                if (createUserResult.Succeeded)
                {
                    _logger.LogInformation("Adding the 'Trial' claim for the user.");

                    // Add the Trial claim
                    Claim trialClaim = new Claim("Trial", DateTime.UtcNow.ToString());  // TODO - check why we need Trial claim
                    await _userManager.AddClaimAsync(newUser, trialClaim);

                    _logger.LogInformation("Adding external login for the user.");

                    // Add external login for the user
                    IdentityResult createLoginResult = await _userManager.AddLoginAsync
                    (
                        newUser, 
                        new ExternalLoginInfo
                        (
                            null,
                            registerModel.LoginProvider,
                            registerModel.ProviderKey,
                            registerModel.ProviderDisplayName
                        )
                    );
                    
                    if (createLoginResult.Succeeded)
                    {
                        _logger.LogInformation($"Updating the newly created user - setting 'EmailConfirmed' to 'true'.");

                        // Email confirmation is not required because we've obtained email from secure source (external provider)
                        newUser.EmailConfirmed = true;
                        await _userManager.UpdateAsync(newUser);

                        _logger.LogInformation($"Trying to sign in user {newUser.Email} with new external login provider.");

                        SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(registerModel.LoginProvider, registerModel.ProviderKey, false);

                        return await AuthenticateAsync(signInResult, newUser, clientId, ExternalLoginStatus.NewExternalLoginAddedSuccess);
                    }
                }

                return GetErrorResult(createUserResult);
            }

            if(string.IsNullOrWhiteSpace(registerModel.AssociateEmail))
            {
                return BadRequest("Associate email is required.");
            }

            IUser existingUser = await _userManager.FindByEmailAsync(registerModel.AssociateEmail);
            if (existingUser != null)
            {
                _logger.LogInformation($"There is user account registered with {registerModel.AssociateEmail} email.");
                _logger.LogInformation($"Email {registerModel.AssociateEmail} is {(existingUser.EmailConfirmed ? "confirmed" : "not confirmed")}.");

                if (!existingUser.EmailConfirmed)
                {
                    return Ok(new AuthenticateResponseApiModel { ExternalLoginStatus = ExternalLoginStatus.EmailRequiresConfirmation });
                }

                string token = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser);

                var callbackUrl = Url.Action
                (
                    "ConfirmExternalProvider",
                    "ExternalLogin",
                    values: new
                    {
                        id = existingUser.Id,
                        clientId,
                        token,
                        loginProvider = registerModel.LoginProvider,
                        loginProviderDisplayName = registerModel.LoginProvider,
                        providerKey = registerModel.ProviderKey
                    },
                    protocol: Request.Scheme
                );

                _logger.LogInformation($"Sending email confirmation token to confirm association of {registerModel.ProviderDisplayName} external login account.");

                // TODO - missing email implementation
                //await _emailSender.SendEmailAsync(existingUser.Email, $"Confirm {registerModel.ProviderDisplayName} external login",
                //    $"Please confirm association of your {registerModel.ProviderDisplayName} account by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>.");

                return Ok(new AuthenticateResponseApiModel { ExternalLoginStatus = ExternalLoginStatus.PendingEmailConfirmation });
            }

            _logger.LogInformation($"There is no user account registered with {registerModel.AssociateEmail} email.");

            return Ok(new AuthenticateResponseApiModel { ExternalLoginStatus = ExternalLoginStatus.UserAccountNotFound });
        }
    }
}