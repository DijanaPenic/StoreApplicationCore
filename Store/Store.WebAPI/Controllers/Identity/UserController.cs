using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Resta.UriTemplates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Common.Extensions;
using Store.Common.Parameters;
using Store.Common.Parameters.Filtering;
using Store.WebAPI.Models;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Constants;
using Store.WebAPI.Infrastructure.Authorization.Attributes;
using Store.WebAPI.Infrastructure.Authorization.Extensions;
using Store.Services.Identity;
using Store.Messaging.Services.Common;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ApplicationControllerBase
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public UserController
        (
            ApplicationUserManager userManager,
            ApplicationRoleManager roleManager,
            ApplicationSignInManager signInManager,
            IAuthorizationService authorizationService,
            ILogger<UserController> logger,
            IMapper mapper,
            IEmailService emailService,
            IQueryUtilityFacade queryUtilityFacade
        ) : base(queryUtilityFacade)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _authorizationService = authorizationService;
            _logger = logger;
            _mapper = mapper;
            _emailService = emailService;
        }

        /// <summary>Retrieves user profile for the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("{userId:guid}/profile")]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserProfileAsync([FromRoute] Guid userId)
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

            IList<UserLoginInfo> logins = await _userManager.FindLoginsAsync(user, true);

            UserProfileGetApiModel userProfileResponse = new UserProfileGetApiModel
            {
                Username = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                ExternalLogins = _mapper.Map<ExternalLoginGetApiModel[]>(logins),
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                TwoFactorClientRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };

            return Ok(userProfileResponse);
        }

        /// <summary>Updates user profile for the currently logged in user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userProfileModel">The user profile model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Authorize]
        [Route("{userId:guid}/profile")]
        [Produces("application/json")]
        public async Task<IActionResult> PatchUserProfileAsync([FromRoute] Guid userId, [FromBody] UserProfilePatchApiModel userProfileModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            if (!IsCurrentUser(userId))
            {
                return Forbid();
            }

            IUser user = await _userManager.FindUserByKeyAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Email != userProfileModel.Email) user.EmailConfirmed = false;

            _mapper.Map(userProfileModel, user);
            IdentityResult userProfileResult = await _userManager.UpdateAsync(user);

            if (!userProfileResult.Succeeded) return BadRequest(userProfileResult.Errors);

            // Need to send email confirmation if email is not confirmed
            if (!user.EmailConfirmed)
            {
                // Get email confirmation token
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                _logger.LogInformation("Email confirmation token has been generated.");

                UriTemplate template = new UriTemplate(userProfileModel.ConfirmationUrl);
                string callbackUrl = template.Resolve(new Dictionary<string, object>
                {
                    { "userId", user.Id.ToString() },
                    { "token", token.Base64ForUrlEncode() }
                });

                _logger.LogInformation("Sending email confirmation email.");

                await _emailService.SendConfirmEmailAsync(GetCurrentUserClientId(), user.Email, callbackUrl, user.UserName);
            }

            return Ok();
        }

        /// <summary>Retrieves external login connections for the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("{userId:guid}/external")]
        [Produces("application/json")]
        public async Task<IActionResult> GetExternalLoginAsync([FromRoute] Guid userId)
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

            IList<UserLoginInfo> logins = await _userManager.FindLoginsAsync(user, true);

            return Ok(_mapper.Map<IList<ExternalLoginGetApiModel>>(logins));
        }

        /// <summary>
        /// Removes any social external connection between user and provider.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="loginProvider">The login provider.</param>
        /// <param name="providerKey">The provider key.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpDelete]
        [Authorize]
        [Route("{userId:guid}/external")]
        public async Task<IActionResult> DisconnectExternalLoginAsync([FromRoute] Guid userId, [FromQuery] string loginProvider, [FromQuery] string providerKey)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }
            if(string.IsNullOrEmpty(loginProvider))
            {
                return BadRequest("Login Provider cannot be empty.");
            }
            if(string.IsNullOrEmpty(providerKey))
            {
                return BadRequest("Provider Key cannot be empty.");
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

            IdentityResult removeLoginResult = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);

            return removeLoginResult.Succeeded ? Ok() : BadRequest(removeLoginResult.Errors);
        }

        /// <summary>Creates the specified user.</summary>
        /// <param name="createUserModel">The create user model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Route("")]
        [Consumes("application/json")]
        [SectionAuthorization(SectionType.User, AccessType.Create)]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserPostApiModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IUser user = _mapper.Map<IUser>(createUserModel);

            IdentityResult userResult = await _userManager.CreateAsync(user, createUserModel.Password);

            if (!userResult.Succeeded) return BadRequest(userResult.Errors);

            _logger.LogInformation("User created a new account with password.");

            // Need to retrieve user to get the updated identifier field
            user = await _userManager.FindByEmailAsync(user.Email);

            IdentityResult roleResult = await _userManager.AddToRolesAsync(user, createUserModel.Roles);

            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            _logger.LogInformation("User assigned to roles.");

            return Created();
        }

        /// <summary>Updates the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userModel">The user model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Route("{userId:guid}")]
        [Consumes("application/json")]
        [SectionAuthorization(SectionType.User, AccessType.Update)]
        public async Task<IActionResult> PatchUserAsync([FromRoute] Guid userId, [FromBody] UserPatchApiModel userModel)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the user we want to update
            IUser user = await _userManager.FindUserByKeyAsync(userId, OptionsFactory.Create(new string[] { nameof(IUser.Roles) }));
            if (user == null)
            {
                return NotFound();
            }
            
            bool isRoleSelectionValid = await _roleManager.IsValidRoleSelectionAsync(userModel.Roles);
            if (!isRoleSelectionValid)
            {
                return BadRequest("Invalid role selection.");
            }

            _mapper.Map(userModel, user);
            IdentityResult userResult = await _userManager.UpdateAsync(user);

            if (!userResult.Succeeded) return BadRequest(userResult.Errors);

            // Remove user from roles that are not listed in model.roles
            IEnumerable<string> rolesToRemove = user.Roles.Select(r => r.Name).Except(userModel.Roles);
            IdentityResult removeRolesResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (!removeRolesResult.Succeeded) return BadRequest(removeRolesResult.Errors);

            // Assign user to roles
            if (userModel.Roles != null)
            {
                IEnumerable<string> rolesToAdd = userModel.Roles.Except(user.Roles.Select(r => r.Name));
                IdentityResult addRolesResult = await _userManager.AddToRolesAsync(user, rolesToAdd);

                if (!addRolesResult.Succeeded) return BadRequest(addRolesResult.Errors);
            }

            return Ok();
        }

        /// <summary>Updates user's activity status to "Locked", i.e. prevents the user from accessing the application.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Route("{userId:guid}/lock")]
        [SectionAuthorization(SectionType.User, AccessType.Update)]
        public async Task<IActionResult> LockUserAsync([FromRoute] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            IUser user = await _userManager.FindUserByKeyAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            DateTime lockoutDate = DateTime.Now.AddYears(50);
            IdentityResult result = await _userManager.SetLockoutEndDateAsync(user, lockoutDate);

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        /// <summary>Updates user's activity status to "Unlocked", i.e. enables user's application access.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Route("{userId:guid}/unlock")]
        [SectionAuthorization(SectionType.User, AccessType.Update)]
        public async Task<IActionResult> UnlockUserAsync([FromRoute] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            IUser user = await _userManager.FindUserByKeyAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(-1));

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        /// <summary>Deletes the user (soft delete).</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpDelete]
        [Route("{userId:guid}/delete")]
        [SectionAuthorization(SectionType.User, AccessType.Delete)]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            IUser user = await _userManager.FindUserByKeyAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.DeleteAsync(user);

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        /// <summary>Updates user's account status to "Approved", i.e. confirms the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Route("{userId:guid}/approve")]
        [SectionAuthorization(SectionType.User, AccessType.Update)]
        public async Task<IActionResult> ApproveUserAsync([FromRoute] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            IUser user = await _userManager.FindUserByKeyAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.ApproveUserAsync(user);

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        /// <summary>Asynchronously updates user's account status to "Disapproved", i.e. refutes the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Route("{userId:guid}/disapprove")]
        [SectionAuthorization(SectionType.User, AccessType.Update)]
        public async Task<IActionResult> DisapproveUserAsync([FromRoute] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            IUser user = await _userManager.FindUserByKeyAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.DisapproveUserAsync(user);

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        /// <summary>Changes the password for the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="changePasswordModel">The change password model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Authorize]
        [Route("{userId:guid}/change-password")]
        [Consumes("application/json")]
        public async Task<IActionResult> ChangeUserPasswordAsync([FromRoute] Guid userId, [FromBody] ChangePasswordPatchApiModel changePasswordModel)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isCurrentUser = IsCurrentUser(userId);
            if(isCurrentUser && string.IsNullOrEmpty(changePasswordModel.OldPassword))
            {
                return BadRequest("Old Password must be provided.");
            }

            bool hasPermissions = IsCurrentUser(userId) || (await _authorizationService.AuthorizeAsync(User, SectionType.User, AccessType.Full)).Succeeded;
            if (!hasPermissions)
            {
                return Forbid();
            }

            IUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User must be logged in.");
            }

            IdentityResult result;

            if (isCurrentUser)
            {
                result = await _userManager.ChangePasswordAsync(user, changePasswordModel.OldPassword, changePasswordModel.NewPassword);
            }
            else // Admin doesn't have to provide old password
            {
                result = await _userManager.ChangePasswordAsync(user, changePasswordModel.NewPassword);
            }

            if (result.Succeeded && changePasswordModel.SendMailNotification)
            {
                _logger.LogInformation("Sending email with password information.");

                await _emailService.SendChangePasswordEmailAsync(GetCurrentUserClientId(), user.Email, user.UserName, changePasswordModel.NewPassword);
            }

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        /// <summary>Sets the password for the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="setPasswordModel">The set password model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Authorize]
        [Route("{userId:guid}/set-password")]
        [Consumes("application/json")]
        public async Task<IActionResult> SetPasswordAsync([FromRoute] Guid userId, [FromBody] SetPasswordPatchApiModel setPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

            // This will set the password only if it's NULL
            IdentityResult result = await _userManager.AddPasswordAsync(user, setPasswordModel.Password);

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        /// <summary>Retrieves users by specified search criteria.</summary>
        /// <param name="showInactive">if set to <c>true</c> [show inactive].</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Produces("application/json")]
        [SectionAuthorization(SectionType.User, AccessType.Read)]
        public async Task<IActionResult> GetUsersAsync([FromQuery] bool showInactive = false,
                                                       [FromQuery] string includeProperties = DefaultParameters.IncludeProperties,
                                                       [FromQuery] string searchString = DefaultParameters.SearchString,
                                                       [FromQuery] int pageNumber = DefaultParameters.PageNumber,
                                                       [FromQuery] int pageSize = DefaultParameters.PageSize,
                                                       [FromQuery] string sortOrder = DefaultParameters.SortOrder)
        {
            IUserFilteringParameters filter = FilteringFactory.Create<IUserFilteringParameters>(searchString);
            filter.ShowInactive = showInactive;

            IPagedList<IUser> users = await _userManager.FindUsersAsync
            (
                filter,
                paging: PagingFactory.Create(pageNumber, pageSize),
                sorting: SortingFactory.Create(ModelMapperHelper.GetSortPropertyMappings<UserGetApiModel, IUser>(_mapper, sortOrder)),
                options: OptionsFactory.Create(ModelMapperHelper.GetPropertyMappings<UserGetApiModel, IUser>(_mapper, includeProperties))
            );

            if (users != null)
            {
                return Ok(_mapper.Map<PagedApiResponse<UserGetApiModel>>(users)); 
            }

            return NoContent();
        }

        /// <summary>Retrieves the user by identifier.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("{userId:guid}")]
        [Produces("application/json")]
        [SectionAuthorization(SectionType.User, AccessType.Read)]
        public async Task<IActionResult> GetUserAsync([FromRoute] Guid userId, [FromQuery] string includeProperties = DefaultParameters.IncludeProperties)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty.");
            }

            IUser user = await _userManager.FindUserByKeyAsync(userId, OptionsFactory.Create(ModelMapperHelper.GetPropertyMappings<UserGetApiModel, IUser>(_mapper, includeProperties)));

            if (user != null)
                return Ok(_mapper.Map<UserGetApiModel>(user));

            return NotFound();
        }

        /// <summary>Assigns roles to the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="rolesToAssign">The roles to assign.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Route("{userId:guid}/roles")]
        [Produces("application/json")]
        [SectionAuthorization(SectionType.User, AccessType.Update)]
        public async Task<IActionResult> AssignRolesToUserAsync([FromRoute] Guid userId, [FromQuery] string[] rolesToAssign)
        {
            if (rolesToAssign?.Length == 0 || userId == Guid.Empty)
            {
                return BadRequest();
            }

            // Find the user we want to assign roles to
            IUser user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Check if provided role selection is valid
            bool isRoleSelectionValid = await _roleManager.IsValidRoleSelectionAsync(rolesToAssign);
            if (!isRoleSelectionValid)
            {
                return BadRequest("Invalid role selection.");
            }

            // Remove user from current roles, if any
            IList<string> currentRoles = await _userManager.GetRolesAsync(user);
            IdentityResult removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded) return BadRequest(removeResult.Errors);

            // Assign user to the new roles
            IdentityResult addResult = await _userManager.AddToRolesAsync(user, rolesToAssign);

            if (!addResult.Succeeded) return BadRequest(addResult.Errors);

            return Ok(new { userId, roles = rolesToAssign });
        }

        /// <summary>Disables the two factor authentication for the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Authorize]
        [Route("{userId:guid}/two-factor/disable")]
        public async Task<IActionResult> DisableUserTwoFactorAsync([FromRoute] Guid userId)
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

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return BadRequest("Cannot disable two factor authentication as it's not currently enabled.");
            }

            IdentityResult result = await _userManager.SetTwoFactorEnabledAsync(user, false);

            _logger.LogInformation("Two factor authentication is disabled for the user.");

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }
    }
}