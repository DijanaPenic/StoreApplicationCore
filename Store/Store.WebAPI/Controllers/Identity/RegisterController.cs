using System;
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
using Store.Common.Helpers;
using Store.Common.Extensions;
using Store.Common.Helpers.Identity;
using Store.Models.Identity;
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
    [Route("api/register")]
    public class RegisterController : ApplicationControllerBase
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IEmailSenderService _emailClientSender;
        private readonly ISmsSenderService _smsSender;
        private readonly ICountriesService _countriesService;
        private readonly ICacheProvider _cacheProvider;

        public RegisterController
        (
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            ILogger<RegisterController> logger,
            IMapper mapper,
            IEmailSenderService emailClientSender,
            ISmsSenderService smsSender,
            ICountriesService countriesService,
            ICacheManager cacheManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _emailClientSender = emailClientSender;
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
        [Consumes("application/json")]
        public async Task<IActionResult> RegisterUserAsync(RegisterPostApiModel registerUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify client information
            if (!Guid.TryParse(registerUserModel.ClientId, out Guid clientId) || GuidHelper.IsNullOrEmpty(clientId))
            {
                return BadRequest($"Client '{clientId}' format is invalid.");
            }

            IUser user = _mapper.Map<IUser>(registerUserModel);
            user.IsApproved = true;

            IdentityResult userResult = await _userManager.CreateAsync(user, registerUserModel.Password);

            if (!userResult.Succeeded) return BadRequest(userResult.Errors);

            _logger.LogInformation($"User [{user.UserName}] has been registered.");

            // Assign user to Guest role
            IdentityResult roleResult = await _userManager.AddToRolesAsync(user, new List<string>() { RoleHelper.Guest });

            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            _logger.LogInformation("Guest role has been assigned to the user.");

            // Save the registration cookie - if the cookie is present in the browser, the user can be automaticalled signed in. Otherwise, the user must be redirected to the Login page.
            await _signInManager.RegisterAsync(user);

            // Get email confirmation token
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            _logger.LogInformation("Email confirmation token has been generated.");

            UriTemplate template = new UriTemplate(registerUserModel.ActivationUrl);
            string callbackUrl = template.Resolve(new Dictionary<string, object>
            {
                { "userId", user.Id.ToString() },
                { "token", token.Base64ForUrlEncode() }
            });

            _logger.LogInformation("Sending account activation email to activate account.");

            await _emailClientSender.SendConfirmAccountAsync(clientId, user.Email, callbackUrl);

            return Ok();
        }

        /// <summary>Either creates a new external login account or establishes the external provider association with the existing account (different email address).</summary>
        /// <param name="registerModel">The external login register model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Identity.External")]
        [Route("external")]
        [Consumes("application/json")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterExternalRequestApiModel registerModel)
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

            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
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

                string firstName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.GivenName);
                string lastName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Surname);
                string email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
                string phoneNumber = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.MobilePhone);

                IUser newUser = new User
                {
                    UserName = registerModel.UserName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    FirstName = firstName,
                    LastName = lastName,
                    IsApproved = true
                };

                _logger.LogInformation($"Creating a new user {email}.");

                // Create a new user
                IdentityResult createUserResult = await _userManager.CreateAsync(newUser);

                if (!createUserResult.Succeeded) return BadRequest(createUserResult.Errors);

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
                        externalLoginInfo.LoginProvider,
                        externalLoginInfo.ProviderKey,
                        externalLoginInfo.ProviderDisplayName
                    )
                );

                if (!createLoginResult.Succeeded) return BadRequest(createLoginResult.Errors);

                _logger.LogInformation($"Updating the newly created user - setting 'EmailConfirmed' to 'true'.");

                // Email confirmation is not required because we've obtained email from secure source (external provider)
                newUser.EmailConfirmed = true;

                // Phone number confirmation is not required because we've obtained phone number from secure source (external provider)
                if (!string.IsNullOrEmpty(newUser.PhoneNumber)) newUser.PhoneNumberConfirmed = true;

                await _userManager.UpdateAsync(newUser);

                return Ok(ExternalAuthStep.AddedNewExternalLogin);
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

                    return Ok(ExternalAuthStep.UserNotAllowed);
                }

                _logger.LogInformation($"Email {registerModel.AssociateEmail} is {(existingUser.EmailConfirmed ? "confirmed" : "not confirmed")}.");

                // Return an error if email is not confirmed
                if (!existingUser.EmailConfirmed)
                {
                    return Ok(ExternalAuthStep.UserEmailNotConfirmed);
                }

                // Otherwise, send a token to confirm association
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser);

                // Add the external provider (confirmed = false)
                IdentityResult createLoginResult = await _userManager.AddOrUpdateLoginAsync(existingUser, externalLoginInfo, token);

                if (!createLoginResult.Succeeded) return BadRequest(createLoginResult.Errors);

                // Send email
                UriTemplate template = new UriTemplate(registerModel.ConfirmationUrl);
                string callbackUrl = template.Resolve(new Dictionary<string, object>
                {
                    { "userId", existingUser.Id.ToString() },
                    { "token", token.Base64ForUrlEncode() }
                });

                _logger.LogInformation($"Sending email confirmation token to confirm association with {externalLoginInfo.ProviderDisplayName} external login account.");

                await _emailClientSender.SendConfirmExternalAccountAsync(clientId, existingUser.Email, callbackUrl, externalLoginInfo.ProviderDisplayName);

                return Ok(ExternalAuthStep.PendingExternalLoginCreation);
            }

            _logger.LogInformation($"There is no user account registered with {registerModel.AssociateEmail} email.");

            return Ok(ExternalAuthStep.UserNotFound);
        }
    }
}