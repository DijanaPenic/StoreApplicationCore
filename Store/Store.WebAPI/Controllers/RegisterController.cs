﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using AutoMapper;
using Resta.UriTemplates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Store.Cache.Common;
using Store.Models.Identity;
using Store.Common.Extensions;
using Store.Common.Helpers.Identity;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.WebAPI.Constants;
using Store.WebAPI.Models.Identity;
using Store.Services.Identity;
using Store.Service.Common.Services;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/register")]
    public class RegisterController : IdentityControllerBase
    {
        private readonly ApplicationUserManager _userManager;
        private readonly SignInManager<IUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IEmailSenderService _emailSender;
        private readonly ISmsSenderService _smsSender;
        private readonly ICountriesService _countriesService;
        private readonly ICacheProvider _cacheProvider;

        public RegisterController
        (
            ApplicationUserManager userManager,
            SignInManager<IUser> signInManager,
            ILogger<RegisterController> logger,
            IMapper mapper,
            IEmailSenderService emailSender,
            ISmsSenderService smsSender,
            ICountriesService countriesService,
            ICacheManager cacheManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _countriesService = countriesService;
            _cacheProvider = cacheManager.CacheProvider;
        } 

        /// <summary>Registers a new user account.</summary>
        /// <param name="registerUserModel">The register user model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("")]
        public async Task<IActionResult> RegisterUserAsync(RegisterPostApiModel registerUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IUser user = _mapper.Map<IUser>(registerUserModel);
            user.IsApproved = true;

            IdentityResult userResult = await _userManager.CreateAsync(user, registerUserModel.Password);

            if (!userResult.Succeeded) return GetErrorResult(userResult);

            _logger.LogInformation($"User [{user.UserName}] has been registered.");

            // Assign user to Guest role
            IdentityResult roleResult = await _userManager.AddToRolesAsync(user, new List<string>() { RoleHelper.Guest });

            if (!roleResult.Succeeded) return GetErrorResult(roleResult);

            _logger.LogInformation("Guest role has been assigned to the user.");

            await SendEmailAsync(user, registerUserModel.ActivationUrl);

            return Ok();
        }

        /// <summary>Generates and sends the email confirmation token.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("{userId:guid}/verify/email")]
        public async Task<IActionResult> SendEmailConfirmationTokenAsync([FromRoute] Guid userId, [FromQuery] string returnUrl)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
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

            await SendEmailAsync(user, returnUrl);

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
        [Route("{userId:guid}/verify/email/token/{token}")]
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

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        /// <summary>Either creates a new external login account or establishes the external provider association with the existing account (different email address).</summary>
        /// <param name="registerModel">The external login register model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Identity.External")]
        [Route("external")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterExternalRequestApiModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExternalLoginInfo loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return BadRequest("External login information is missing.");
            }

            // Create a new account
            if (!registerModel.AssociateExistingAccount)
            {
                if (string.IsNullOrWhiteSpace(registerModel.UserName))
                {
                    return BadRequest("Username is required.");
                }

                string firstName = loginInfo.Principal.FindFirstValue(ClaimTypes.GivenName);
                string lastName = loginInfo.Principal.FindFirstValue(ClaimTypes.Surname);
                string email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);

                IUser newUser = new User
                {
                    UserName = registerModel.UserName,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    IsApproved = true
                };

                _logger.LogInformation($"Creating a new user {email}.");

                // Create a new user
                IdentityResult createUserResult = await _userManager.CreateAsync(newUser);

                if (!createUserResult.Succeeded) return GetErrorResult(createUserResult);

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
                        loginInfo.LoginProvider,
                        loginInfo.ProviderKey,
                        loginInfo.ProviderDisplayName
                    )
                );

                if (!createLoginResult.Succeeded) return GetErrorResult(createLoginResult);

                _logger.LogInformation($"Updating the newly created user - setting 'EmailConfirmed' to 'true'.");

                // Email confirmation is not required because we've obtained email from secure source (external provider)
                newUser.EmailConfirmed = true;
                await _userManager.UpdateAsync(newUser);

                return Ok(ExternalLoginStep.NewExternalLoginAddedSuccess);
            }

            // Associate with the existing account
            if (string.IsNullOrWhiteSpace(registerModel.AssociateEmail))
            {
                return BadRequest("Associate email is required.");
            }

            IUser existingUser = await _userManager.FindByEmailAsync(registerModel.AssociateEmail);
            if (existingUser != null)
            {
                _logger.LogInformation($"There is a user account registered with {registerModel.AssociateEmail} email.");

                if (existingUser.IsDeleted || !existingUser.IsApproved)
                {
                    _logger.LogInformation("User is deleted or not approved.");

                    return Ok(ExternalLoginStep.UserNotAllowed);
                }

                _logger.LogInformation($"Email {registerModel.AssociateEmail} is {(existingUser.EmailConfirmed ? "confirmed" : "not confirmed")}.");

                // Return an error if email is not confirmed
                if (!existingUser.EmailConfirmed)
                {
                    return Ok(ExternalLoginStep.EmailRequiresConfirmation);
                }

                // Otherwise, send a token to confirm association
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser);

                // Add the external provider (confirmed = false)
                IdentityResult createLoginResult = await _userManager.AddOrUpdateLoginAsync(existingUser, loginInfo, token);

                if (!createLoginResult.Succeeded) return GetErrorResult(createLoginResult);

                // Send email
                UriTemplate template = new UriTemplate(registerModel.ConfirmationUrl);
                string callbackUrl = template.Resolve(new Dictionary<string, object>
                {
                    { "userId", existingUser.Id.ToString() },
                    { "token", token.Base64ForUrlEncode() }
                });

                _logger.LogInformation($"Sending email confirmation token to confirm association with {loginInfo.ProviderDisplayName} external login account.");

                await _emailSender.SendEmailAsync
                 (
                     existingUser.Email,
                     $"Confirm {loginInfo.ProviderDisplayName} external login - Store Application",
                     $"Please confirm association with {loginInfo.ProviderDisplayName} account by clicking<br> <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Confirm External Login</a>."
                 );

                return Ok(ExternalLoginStep.PendingEmailConfirmation);
            }

            _logger.LogInformation($"There is no user account registered with {registerModel.AssociateEmail} email.");

            return Ok(ExternalLoginStep.UserAccountNotFound);
        }

        /// <summary>Confirms the external provider association by confirming the user's email.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost] 
        [AllowAnonymous]
        [Route("external/{userId:guid}/verify/email/token/{token}")]
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

                if (!emailConfirmationResult.Succeeded)
                    return GetErrorResult(emailConfirmationResult);
            }

            // Create a new external login for the user
            IdentityResult confirmLoginResult = await _userManager.ConfirmLoginAsync(user, decodedToken);

            if (!confirmLoginResult.Succeeded) return GetErrorResult(confirmLoginResult);

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
        [Route("{userId:guid}/verify/sms")]
        public async Task<IActionResult> SendPhoneNumberConfirmationTokenAsync([FromRoute] Guid userId, PhoneNumberVerifyPostApiModel phoneNumberVerifyModel)
        {
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
            if(user.PhoneNumberConfirmed)
            {
                return BadRequest("Phone number is already confirmed.");
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
        [Route("{userId:guid}/verify/sms/token/{token}")]
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

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        private async Task SendEmailAsync(IUser user, string activationUrl)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(activationUrl))
                throw new ArgumentNullException(nameof(activationUrl));

            // Get email confirmation token
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            _logger.LogInformation("Email confirmation token has been generated.");

            UriTemplate template = new UriTemplate(activationUrl);
            string callbackUrl = template.Resolve(new Dictionary<string, object>
            {
                { "userId", user.Id.ToString() },
                { "token", token.Base64ForUrlEncode() }
            });

            _logger.LogInformation("Sending account activation email to activate account.");

            await _emailSender.SendEmailAsync
            (
                user.Email,
                "Welcome to Store! Confirm Your Email",
                $"You're on your way! Let's confirm your email address.<br> By clicking on the following link, you are confirming your email address.<br> <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Confirm Email Address</a>."
            );
        }
    }
}