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
using Store.Models.Api.Identity;
using Store.Model.Common.Models.Identity;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

// Blog post: https://chsakell.com/2019/07/28/asp-net-core-identity-series-external-provider-authentication-registration-strategy/
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

        [HttpGet]
        [AllowAnonymous]
        [Route("authentiate")] // TODO - need to use model from body instead of query parameters
        public IActionResult Authenticate([FromQuery] string provider, [FromQuery] Guid clientId, [FromQuery] string clientSecret = null)
        {
            string redirectUrl = Url.Action("Callback", "ExternalLogin", new { clientId, clientSecret });        // Url.Action is not working if "CallbackAsync" value is used
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("authentiate-callback")]
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

            // Sign in the user with this external login provider if the user already has a login.
            IUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user != null)
            {
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
                // RULE #5 
                if (!user.EmailConfirmed)
                {
                    string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    string callbackUrl = Url.Action
                    (
                        "ConfirmExternalProvider",
                        "ExternalLogin",
                        values: new
                        {
                            userId = user.Id,
                            code = token,
                            loginProvider = info.LoginProvider,
                            providerDisplayName = info.LoginProvider,
                            providerKey = info.ProviderKey
                        },
                        protocol: Request.Scheme)
                    ;

                    // TODO - missing email implementation
                    //await _emailSender.SendEmailAsync
                    //(
                    //    user.Email, 
                    //    $"Confirm {info.ProviderDisplayName} external login",
                    //    $"Please confirm association of your {info.ProviderDisplayName} account by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>."
                    //);

                    return Ok(new AuthenticateResponseApiModel { ExternalLoginStatus = ExternalLoginStatus.PendingEmailConfirmation } );
                }

                // Add the external provider
                await _userManager.AddLoginAsync(user, info);

                SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                return await AuthenticateAsync(signInResult, user, clientId, ExternalLoginStatus.NewExternalLoginAddedSuccess);
            }

            string registerUrl = Url.Action
            (
                "Register",
                "ExternalLoginAccount",
                values: new
                {
                    associate = userEmail,
                    loginProvider = info.LoginProvider,
                    providerDisplayName = info.ProviderDisplayName,
                    providerKey = info.ProviderKey
                },
                protocol: Request.Scheme)
            ;

            return new RedirectResult(registerUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("users/{id:guid}/confirm-external-provider")]
        public async Task<IActionResult> ConfirmExternalProvider([FromRoute] Guid id, Guid clientId, string loginProvider, string providerDisplayName, string providerKey)
        {
            if (GuidHelper.IsNullOrEmpty(id))
            {
                return BadRequest("User Id is missing.");
            }
            if (GuidHelper.IsNullOrEmpty(clientId))
            {
                return BadRequest("Client Id is missing.");
            }
            if (GuidHelper.IsNullOrEmpty(loginProvider))
            {
                return BadRequest("Login provider is required.");
            }
            if (GuidHelper.IsNullOrEmpty(providerDisplayName))
            {
                return BadRequest("Provider display name is required.");
            }
            if (GuidHelper.IsNullOrEmpty(providerKey))
            {
                return BadRequest("Provider key is required.");
            }

            string token = Request.Headers["Token"].First();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing.");
            }

            IUser user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                NotFound();
            }

            // This comes from an external provider so we can confirm the email as well
            IdentityResult confirmationResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmationResult.Succeeded)
                return InternalServerError();

            IdentityResult newLoginResult = await _userManager.AddLoginAsync(user, new ExternalLoginInfo(null, loginProvider, providerKey, providerDisplayName));
            if (!newLoginResult.Succeeded)
                return InternalServerError();

            SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent: false, bypassTwoFactor: true);
            return await AuthenticateAsync(signInResult, user, clientId, ExternalLoginStatus.NewExternalLoginAddedSuccess);
        }

        [HttpPost]
        [Route("associate")]
        // TODO - authentication and implementation
        public void Associate()
        {
        }
    }
}