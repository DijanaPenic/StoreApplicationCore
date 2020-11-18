using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

using Store.Common.Helpers;
using Store.Common.Helpers.Identity;
using Store.WebAPI.Identity;
using Store.Models.Identity;
using Store.Models.Api.Identity;
using Store.Model.Common.Models.Identity;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

// Blog post: https://chsakell.com/2019/07/28/asp-net-core-identity-series-external-provider-authentication-registration-strategy/

// Rule #1: Users can sign in only if their email is confirmed.
// Rule #2: Users authenticated by an external provider are considered trusted.
// Rule #3: Users authenticated by an external provider that select the option to associate the email address used in the provider with an existing account registered with a different email address, will have to confirm the external provider association through the existing account’s email address.
// Rule #4: An association can only happen with already confirmed accounts.
// Rule #5: Users authenticated by an external provider but have an existing account with the same email address that hasn’t been confirmed, have to confirm the association which eventually will automatically confirm the existing account as well. 

// Authentication:
//
// External login already exists - authenticate
// External login doesn't exist:
//          The same email address is found:
//              * email is confirmed - add external login and authenticate
//              * email is not confirmed - send token to the email
//          The same email address is not found - user can choose to register a new account or to associate external login with the existing account (different email)
//              * register a new account - the system will create a new user and authenticate
//              * associate with the existing account:
//                  * email is confirmed - send token to the email
//                  * email is not confirmed - return an error
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

        /// <summary>Retrieves the names of the supported external login providers.</summary>
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
        /// <param name="authenticateModel">The authenticate external model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost] 
        [AllowAnonymous]
        [Route("initiate-authentication")] 
        public async Task<IActionResult> InitiateAuthentictionAsync(AuthenticateExternalRequestApiModel authenticateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable<AuthenticationScheme> schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            AuthenticationScheme provider = schemes.Where(el => el.Name.ToUpper().Equals(authenticateModel.Provider.ToUpper())).FirstOrDefault();

            if(provider == null)
            {
                return BadRequest("Not supported external login provider.");
            }

            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider.Name, authenticateModel.ReturnUrl);

            return Challenge(properties, provider.Name);
        }

        /// <summary>Authenticates the user via the external login request.</summary>
        /// <param name="authenticateModel">The authenticate external model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]   
        [Authorize(AuthenticationSchemes = "Identity.External")]
        [Route("authentiate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticateExternalCallbackRequestApiModel authenticateModel)
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

        /// <summary>Confirms the external provider association.</summary>
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
            return await AuthenticateAsync(signInResult, user, clientId, ExternalLoginStatus.NewExternalLoginAddedSuccess, loginProvider);
        }

        /// <summary>Either creates a new external login account or establishes the external provider association with the existing account (different email address).</summary>
        /// <param name="registerModel">The external login register model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Identity.External")]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] ExternalRegisterRequestApiModel registerModel)
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

            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest("External login information is missing.");
            }

            // Create a new account
            if (!registerModel.AssociateExistingAccount)
            {
                if(string.IsNullOrWhiteSpace(registerModel.Username))
                {
                    return BadRequest("Username is required.");
                }

                string firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                string lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
                string email = info.Principal.FindFirstValue(ClaimTypes.Email);

                IUser newUser = new User 
                { 
                    UserName = registerModel.Username, 
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    IsApproved = true
                };

                _logger.LogInformation($"Creating a new user {email}.");

                // Create a new user
                IdentityResult createUserResult = await _userManager.CreateAsync(newUser);
                if (createUserResult.Succeeded)
                {
                    _logger.LogInformation("Adding Guest role to the user.");

                    await _userManager.AddToRolesAsync(newUser, new List<string>() { RoleHelper.Guest });

                    _logger.LogInformation("Adding external login for the user.");

                    // Add external login for the user
                    IdentityResult createLoginResult = await _userManager.AddLoginAsync
                    (
                        newUser, 
                        new ExternalLoginInfo
                        (
                            null,
                            info.LoginProvider,
                            info.ProviderKey,
                            info.ProviderDisplayName
                        )
                    );
                    
                    if (createLoginResult.Succeeded)
                    {
                        _logger.LogInformation($"Updating the newly created user - setting 'EmailConfirmed' to 'true'.");

                        // Email confirmation is not required because we've obtained email from secure source (external provider)
                        newUser.EmailConfirmed = true;
                        await _userManager.UpdateAsync(newUser);

                        _logger.LogInformation($"Trying to sign in user {newUser.Email} with new external login provider.");

                        SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

                        return await AuthenticateAsync(signInResult, newUser, clientId, ExternalLoginStatus.NewExternalLoginAddedSuccess, info.LoginProvider);
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
                        userId = existingUser.Id,
                        clientId,
                        token,
                        loginProvider = info.LoginProvider,
                        loginProviderDisplayName = info.ProviderDisplayName,
                        providerKey = info.ProviderKey
                    },
                    protocol: Request.Scheme
                );

                _logger.LogInformation($"Sending email confirmation token to confirm association of {info.ProviderDisplayName} external login account.");

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