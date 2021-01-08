using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Resta.UriTemplates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Store.Cache.Common;
using Store.Common.Extensions;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.WebAPI.Constants;
using Store.WebAPI.Models.Identity;
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
        private readonly ILogger _logger;
        private readonly IEmailSenderService _emailClientSender;
        private readonly ISmsSenderService _smsSender;
        private readonly ICountriesService _countriesService;
        private readonly ICacheProvider _cacheProvider;

        public AccountVerifyController
        (
            ApplicationUserManager userManager,
            ILogger<RegisterController> logger,
            IEmailSenderService emailClientSender,
            ISmsSenderService smsSender,
            ICountriesService countriesService,
            ICacheManager cacheManager
        )
        {
            _userManager = userManager;
            _logger = logger;
            _emailClientSender = emailClientSender;
            _smsSender = smsSender;
            _countriesService = countriesService;
            _cacheProvider = cacheManager.CacheProvider;
        } 

        /// <summary>Generates and sends the email confirmation token.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("{userId:guid}/email")]
        public async Task<IActionResult> SendEmailConfirmationTokenAsync([FromRoute] Guid userId, [FromQuery] Guid clientId, [FromQuery] string returnUrl)
        {
			// TODO - potentially check if user is authenticated OR pending verification on login
			
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            if (clientId == Guid.Empty)
            {
                return BadRequest("Client Id cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return BadRequest("Return URL cannot be empty.");
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

            UriTemplate template = new UriTemplate(returnUrl);
            string callbackUrl = template.Resolve(new Dictionary<string, object>
            {
                { "userId", user.Id.ToString() },
                { "token", token.Base64ForUrlEncode() }
            });

            _logger.LogInformation("Sending email confirmation email.");

            await _emailClientSender.SendConfirmAccountEmailAsync(clientId, user.Email, callbackUrl); 

            return Ok();
        }

        /// <summary>Confirms the user's email.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("{userId:guid}/email/token/{token}")]
        public async Task<IActionResult> ConfirmUserEmailAsync([FromRoute] Guid userId, [FromRoute] string token)
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
        [AllowAnonymous]
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

        /// <summary>Generates the phone number confirmation token and sends it via SMS to the phone number.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="phoneNumberVerifyModel">The phone number verify model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("{userId:guid}/sms")]
        [Consumes("application/json")]
        public async Task<IActionResult> SendPhoneNumberConfirmationTokenAsync([FromRoute] Guid userId, PhoneNumberVerifyPostApiModel phoneNumberVerifyModel)
        {
			// TODO - potentially check if user is authenticated OR pending verification on login
			
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

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

            string phoneNumber = string.Concat(phoneNumberVerifyModel.CountryCodeNumber, phoneNumberVerifyModel.PhoneNumber);

            // Get sms confirmation token
            string token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber.GetDigits());

            _logger.LogInformation("Sending SMS confirmation token to activate the account.");

            await _smsSender.SendSmsAsync(phoneNumber, $"Your Store verification code is: {token}.");

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
        [AllowAnonymous]
        [Route("{userId:guid}/sms/token/{token}")]
        public async Task<IActionResult> ConfirmPhoneNumberAsync([FromRoute] Guid userId, [FromRoute] string token, [FromQuery] string phoneNumber)
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
    }
}