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

using Store.Cache.Common;
using Store.Common.Enums;
using Store.Common.Extensions;
using Store.Common.Parameters;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.WebAPI.Constants;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Infrastructure.Authorization.Attributes;
using Store.WebAPI.Infrastructure.Authorization.Extensions;
using Store.Services.Identity;
using Store.Service.Common.Services;
using Store.Messaging.Services.Common;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/account-verify")]
    public class AccountVerifyController : ApplicationControllerBase
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationAuthManager _authManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger _logger;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IVoiceService _voiceService;
        private readonly ICountriesService _countriesService;
        private readonly ICacheProvider _cacheProvider;

        public AccountVerifyController
        (
            ApplicationUserManager userManager,
            ApplicationAuthManager authManager,
            ApplicationSignInManager signInManager,
            IAuthorizationService authorizationService,
            ILogger<RegisterController> logger,
            IEmailService emailService,
            ISmsService smsService,
            IVoiceService voiceService,
            ICountriesService countriesService,
            ICacheManager cacheManager,
            IQueryUtilityFacade queryUtilityFacade
        ) : base(queryUtilityFacade)
        {
            _userManager = userManager;
            _authManager = authManager;
            _signInManager = signInManager;
            _authorizationService = authorizationService;
            _logger = logger;
            _emailService = emailService;
            _smsService = smsService;
            _voiceService = voiceService;
            _countriesService = countriesService;
            _cacheProvider = cacheManager.CacheProvider;
        }

        /// <summary>Generates and sends an email confirmation token to email address associated with user account.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="emailConfirmationModel">The email confirmation model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("{userId:guid}/email")]
        public async Task<IActionResult> SendEmailConfirmationTokenAsync([FromRoute] Guid userId, [FromBody] EmailConfirmationPostApiModel emailConfirmationModel)
        {
            AuthenticateResult authResult = await AuthenticateUserAsync(userId);
            if (authResult.Action != null) return authResult.Action;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IUser user = await _userManager.FindUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            if (user.EmailConfirmed)
            {
                return BadRequest("Email is already confirmed.");
            }

            // Get email confirmation token
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            _logger.LogInformation("Email confirmation token has been generated.");

            UriTemplate template = new UriTemplate(emailConfirmationModel.ReturnUrl);
            string callbackUrl = template.Resolve(new Dictionary<string, object>
            {
                { "userId", user.Id.ToString() },
                { "token", token.Base64ForUrlEncode() }
            });

            _logger.LogInformation("Sending email confirmation email.");

            await _emailService.SendConfirmEmailAsync(authResult.ClientId, user.Email, callbackUrl, user.UserName); 

            return Ok();
        }

        /// <summary>Confirms the user's email.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [ClientAuthorization]
        [Route("{userId:guid}/email/token/{token}")]
        public async Task<IActionResult> ConfirmEmailAsync([FromRoute] Guid userId, [FromRoute] string token)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Token is required.");
            }

            IUser user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                NotFound();
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token.Base64ForUrlDecode());

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        /// <summary>Confirms the external provider association by confirming the user's email.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost] 
        [ClientAuthorization]
        [Route("external/{userId:guid}/email/token/{token}")]
        public async Task<IActionResult> ConfirmExternalProviderAsync([FromRoute] Guid userId, [FromRoute] string token)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Token is required.");
            }

            IUser user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            string decodedToken = token.Base64ForUrlDecode();

            if (!user.EmailConfirmed)
            {
                // External provider is authenticated source so we can confirm the email
                IdentityResult emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (!emailConfirmationResult.Succeeded) return BadRequest(emailConfirmationResult.Errors);
            }

            // Create a new external login for the user
            IdentityResult confirmLoginResult = await _userManager.ConfirmLoginAsync(user, decodedToken);

            if (!confirmLoginResult.Succeeded) return BadRequest(confirmLoginResult.Errors);

            return Ok();
        }

        /// <summary>Generates the phone number confirmation token and sends it via SMS or voice call to the phone number.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="phoneNumberVerifyModel">The phone number verify model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("{userId:guid}/phone-number")]
        [Consumes("application/json")]
        public async Task<IActionResult> SendPhoneNumberConfirmationTokenAsync([FromRoute] Guid userId, [FromBody] PhoneNumberVerifyPostApiModel phoneNumberVerifyModel)
        {
            AuthenticateResult authResult = await AuthenticateUserAsync(userId);
            if (authResult.Action != null) return authResult.Action;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IUser user = await _userManager.FindUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Retrieve countries lookup from cache or the database
            IList<ICountry> countries = await _cacheProvider.GetOrAddAsync
            (
                CacheParameters.Keys.AllCountries,
                () => _countriesService.GetCountriesAsync(),
                DateTimeOffset.MaxValue,
                CacheParameters.Groups.Settings
            );

            if(countries == null)
            {
                return InternalServerError();
            }

            ICountry country = countries.Where
            (c => 
                c.AlphaThreeCode == phoneNumberVerifyModel.IsoCountryCode && 
                c.CallingCodes.Contains(phoneNumberVerifyModel.CountryCodeNumber.Trim('+'))
            ).SingleOrDefault();

            if (country == null)
            {
                return NotFound("Country not found.");
            }

            _logger.LogInformation("Generating phone number confirmation token.");

            string phoneNumber = string.Concat(phoneNumberVerifyModel.CountryCodeNumber, phoneNumberVerifyModel.PhoneNumber.GetDigits());

            // Get confirmation token
            string token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

            _logger.LogInformation("Sending confirmation token to activate the account.");

            if (phoneNumberVerifyModel.IsVoiceCall)
            {
                await _voiceService.CallAsync(phoneNumber, GetAbsoluteUri(Url.RouteUrl(RouteNames.TwilioPhoneNumberVerificationToken, new { token })));
            }
            else
            {
                await _smsService.SendSmsAsync(phoneNumber, $"Your Store verification code is: {token}.");
            }

            return Ok();
        }

        /// <summary>Confirms the phone number by checking the provided confirmation token.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The token.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [ClientAuthorization]
        [Route("{userId:guid}/phone-number/{phoneNumber}/token/{token}")]
        public async Task<IActionResult> ConfirmPhoneNumberAsync([FromRoute] Guid userId, [FromRoute] string token, [FromRoute] string phoneNumber)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            IUser user = await _userManager.FindUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.ChangePhoneNumberAsync(user, phoneNumber.GetDigits(), token);

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        /// <summary>Checks if user is logged in (current user or authorized user) OR pending verification on login.</summary>
        private async Task<AuthenticateResult> AuthenticateUserAsync(Guid userId)
        {
            AuthenticateResult result = new AuthenticateResult();

            if (userId == Guid.Empty)
            {
                result.Action = BadRequest("User Id cannot be empty.");
            }

            string accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                // Retrieve user account information from token
                ClaimsPrincipal claimsPrincipal = await _authManager.ValidateAccessTokenAsync(accessToken);
                if (claimsPrincipal == null)
                {
                    result.Action = Unauthorized("Invalid access token!");
                }
                else
                {
                    bool hasPermissions = IsUser(userId, claimsPrincipal) || (await _authorizationService.AuthorizeAsync(User, SectionType.User, AccessType.Full)).Succeeded;
                    if (!hasPermissions)
                    {
                        result.Action = Forbid();
                    }

                    result.ClientId = GetClientId(claimsPrincipal);
                }
            }
            else
            {
                // Retrieve user account information from cookie
                AccountVerificationInfo accountVerificationInfo = await _signInManager.GetAccountVerificationInfoAsync();
                if (accountVerificationInfo == null)
                {
                    result.Action = Unauthorized("Account information not found.");
                }
                else
                {
                    if (Guid.Parse(accountVerificationInfo.UserId) != userId)
                    {
                        result.Action = Forbid();
                    }

                    result.ClientId = Guid.Parse(accountVerificationInfo.ClientId);
                }
            }

            return result;
        }

        public class AuthenticateResult
        {
            public IActionResult Action { get; set; }

            public Guid ClientId { get; set; }
        }
    }
}