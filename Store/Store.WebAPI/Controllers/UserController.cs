using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Store.Models.Api;
using Store.Models.Api.Identity;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.WebAPI.Identity;
using Store.WebAPI.Constants;
using Store.WebAPI.Infrastructure;
using Store.Common.Helpers;
using Store.Common.Helpers.Identity;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : IdentityControllerBase
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly SignInManager<IUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public UserController
        (
            ApplicationUserManager userManager,
            ApplicationRoleManager roleManager,
            SignInManager<IUser> signInManager,
            ILogger<UserController> logger,
            IMapper mapper
        )
        : base(userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>Retrieves user profile for the currently logged in user.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("profile")]
        public async Task<IActionResult> GetUserProfileAsync()
        {
            // Get currently logged in user
            IUser user = await GetLoggedInUserAsync();

            IList<UserLoginInfo> logins = await _userManager.GetLoginsAsync(user);

            UserProfileGetApiModel userProfileResponse = new UserProfileGetApiModel
            {
                Username = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                ExternalLogins = logins.Select(login => login.ProviderDisplayName).ToList(),
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                TwoFactorClientRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };

            return Ok(userProfileResponse);
        }

        /// <summary>Creates the specified user.</summary>
        /// <param name="createUserModel">The create user model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("")]
        public async Task<IActionResult> CreateUserAsync(UserCreatePostApiModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IUser user = _mapper.Map<IUser>(createUserModel);
            IdentityResult userResult = await _userManager.CreateAsync(user, createUserModel.Password);

            if (!userResult.Succeeded) return GetErrorResult(userResult);

            _logger.LogInformation("User created a new account with password.");

            IdentityResult roleResult = await _userManager.AddToRolesAsync(user, createUserModel.Roles);

            if (!roleResult.Succeeded) return GetErrorResult(roleResult);

            _logger.LogInformation("User assigned to roles.");

            return Ok();
        }

        /// <summary>Updates the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userModel">The user model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("{userId:guid}")]
        public async Task<IActionResult> PatchUserAsync([FromRoute] Guid userId, [FromBody] UserPatchApiModel userModel)
        {
            if (userId == Guid.Empty)
                return BadRequest("User Id is missing.");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isRoleSelectionValid = await _roleManager.IsValidRoleSelectionAsync(userModel.Roles);
            if (!isRoleSelectionValid)
            {
                return BadRequest("Invalid role selection.");
            }

            // Find the user we want to update
            IUser user = await _userManager.FindUserByIdAsync(userId, nameof(IUser.Roles));
            if (user == null)
            {
                return NotFound();
            }

            _mapper.Map(userModel, user);
            IdentityResult userResult = await _userManager.UpdateAsync(user);

            if (!userResult.Succeeded) return GetErrorResult(userResult);

            // Remove user from roles that are not listed in model.roles
            IEnumerable<string> rolesToRemove = user.Roles.Where(r => !userModel.Roles.Contains(r.Name)).Select(r => r.Name);
            IdentityResult removeRolesResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (!removeRolesResult.Succeeded) return GetErrorResult(removeRolesResult);

            // Assign user to roles
            if (userModel.Roles != null)
            {
                IEnumerable<string> rolesToAdd = userModel.Roles.Except(user.Roles.Select(r => r.Name));
                IdentityResult addRolesResult = await _userManager.AddToRolesAsync(user, rolesToAdd);

                if (!addRolesResult.Succeeded) return GetErrorResult(addRolesResult);
            }

            return Ok();
        }

        /// <summary>Unlocks the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("{userId:guid}/unlock")]
        public async Task<IActionResult> UnlockUserAsync([FromRoute] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("User Id is missing.");

            IUser user = await _userManager.FindUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(-1));

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        /// <summary>Changes the user's password.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="changePasswordModel">The change password model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("{userId:guid}/change-password")]
        public async Task<IActionResult> ChangeUserPasswordAsync([FromRoute] Guid userId, ChangePasswordPostApiModel changePasswordModel)
        {
            if (userId == Guid.Empty)
                return BadRequest("User Id is missing.");

            IUser user = await _userManager.FindUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, changePasswordModel.OldPassword, changePasswordModel.NewPassword);

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        /// <summary>Sets the password for currently logged in user.</summary>
        /// <param name="setPasswordModel">The set password model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Authorize]
        [Route("set-password")]
        public async Task<IActionResult> SetPasswordAsync(SetPasswordPostApiModel setPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IUser user = await _userManager.GetUserAsync(User);

            // This will set the password only if it's NULL
            IdentityResult result = await _userManager.AddPasswordAsync(user, setPasswordModel.Password);

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        /// <summary>Retrieves users by specified search criteria.</summary>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="showInactive">if set to <c>true</c> [show inactive].</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="isDescendingSortOrder">if set to <c>true</c> [is descending sort order].</param>
        /// <param name="sortOrderProperty">The sort order property.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("")]
        public async Task<IActionResult> GetUsersAsync([FromQuery] string[] includeProperties, bool showInactive = false, string searchString = DefaultParameters.SearchString, int pageNumber = DefaultParameters.PageNumber,
                                                       int pageSize = DefaultParameters.PageSize, bool isDescendingSortOrder = DefaultParameters.IsDescendingSortOrder, string sortOrderProperty = nameof(UserGetApiModel.UserName))
        {
            IPagedEnumerable<IUser> users = await _userManager.FindUsersAsync
            (
                searchString,
                showInactive,
                ModelMapperHelper.GetPropertyMapping<UserGetApiModel, IUser>(_mapper, sortOrderProperty),
                isDescendingSortOrder,
                pageNumber,
                pageSize,
                ModelMapperHelper.GetPropertyMappings<UserGetApiModel, IUser>(_mapper, includeProperties)
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
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("{userId:guid}")]
        public async Task<IActionResult> GetUserAsync([FromRoute] Guid userId, [FromQuery] string[] includeProperties)
        {
            if (userId == Guid.Empty)
                return BadRequest();

            IUser user = await _userManager.FindUserByIdAsync(userId, ModelMapperHelper.GetPropertyMappings<UserGetApiModel, IUser>(_mapper, includeProperties));

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
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("{userId:guid}/roles")]
        public async Task<IActionResult> AssignRolesToUserAsync([FromRoute] Guid userId, string[] rolesToAssign)
        {
            if (rolesToAssign == null || rolesToAssign.Length == 0 || userId == Guid.Empty)
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

            if (!removeResult.Succeeded) return GetErrorResult(removeResult);

            // Assign user to the new roles
            IdentityResult addResult = await _userManager.AddToRolesAsync(user, rolesToAssign);

            if (!addResult.Succeeded) return GetErrorResult(addResult);

            return Ok(new { userId = userId, roles = rolesToAssign });
        }

        /// <summary>Disables the two factor authentication for the currently logged in user.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [Authorize]
        [Route("two-factor/disable")]
        public async Task<IActionResult> DisableUserTwoFactorAsync()
        {
            // Get currently logged in user
            IUser user = await GetLoggedInUserAsync();

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return BadRequest("Cannot disable two factor authentication as it's not currently enabled.");
            }

            IdentityResult result = await _userManager.SetTwoFactorEnabledAsync(user, false);

            _logger.LogInformation("Two factor authentication is disabled for the user.");

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }
    }
}