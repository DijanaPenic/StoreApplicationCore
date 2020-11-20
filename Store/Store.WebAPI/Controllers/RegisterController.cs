﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Store.Common.Helpers;
using Store.Common.Helpers.Identity;
using Store.Common.Extensions;
using Store.WebAPI.Identity;
using Store.Models.Identity;
using Store.Models.Api.Identity;
using Store.Model.Common.Models.Identity;

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

        public RegisterController
        (
            ApplicationUserManager userManager,
            SignInManager<IUser> signInManager,
            ILogger<RegisterController> logger,
            IMapper mapper
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
        } 

        /// <summary>Registers a new user account.</summary>
        /// <param name="registerUserModel">The register user model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("")]
        public async Task<IActionResult> RegisterUserAsync(UserRegisterPostApiModel registerUserModel)
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

            // Get email confirmation token
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            _logger.LogInformation("Email confirmation token has been generated.");

            TokenResponseApiModel registerResponse = new TokenResponseApiModel
            {
                UserId = user.Id,
                ConfirmationToken = token.Base64ForUrlEncode()
            };

            return Ok(registerResponse);
        }

        /// <summary>Activates user account by confirming the user's email.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [AllowAnonymous]
        [Route("{userId:guid}/confirm-email")]
        public async Task<IActionResult> ConfirmUserEmailAsync([FromRoute] Guid userId) // TODO - token shouldn't be in header + change email confirmation implementation
        {
            if (GuidHelper.IsNullOrEmpty(userId))
            {
                return BadRequest("User Id is missing.");
            }

            string token = Request.Headers["Token"].First();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing.");
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
                if (string.IsNullOrWhiteSpace(registerModel.Username))
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

                        return Ok(ExternalLoginStatus.NewExternalLoginAddedSuccess);
                    }
                }

                return GetErrorResult(createUserResult);
            }

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

                    return Ok(new AuthenticateResponseApiModel { ExternalLoginStatus = ExternalLoginStatus.UserNotAllowed });
                }

                _logger.LogInformation($"Email {registerModel.AssociateEmail} is {(existingUser.EmailConfirmed ? "confirmed" : "not confirmed")}.");

                if (!existingUser.EmailConfirmed)
                {
                    return Ok(ExternalLoginStatus.EmailRequiresConfirmation);
                }

                string token = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser);

                string callbackUrl = Url.Action
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

                return Ok(ExternalLoginStatus.PendingEmailConfirmation);
            }

            _logger.LogInformation($"There is no user account registered with {registerModel.AssociateEmail} email.");

            return Ok(ExternalLoginStatus.UserAccountNotFound);
        }

        /// <summary>Confirms the external provider association by confirming the user's email.</summary>
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
        [Route("external/{userId:guid}/confirm-email")]
        public async Task<IActionResult> ConfirmUserEmailAsync([FromRoute] Guid userId, [FromQuery] Guid clientId, [FromQuery] string token,
        [FromQuery] string loginProvider, [FromQuery] string loginProviderDisplayName, [FromQuery] string providerKey)
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

            return Ok();
        }
    }
}