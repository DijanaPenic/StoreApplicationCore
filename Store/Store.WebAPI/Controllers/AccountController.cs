using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Store.Models.Api;
using Store.Models.Api.Identity;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Cache.Common;
using Store.WebAPI.Models;
using Store.WebAPI.Identity;
using Store.WebAPI.Constants;
using Store.WebAPI.Infrastructure;
using Store.Common.Helpers;
using Store.Common.Helpers.Identity;
using Store.Common.Extensions;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : IdentityControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICacheProvider _cacheProvider;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly ApplicationAuthManager _authManager;
        private readonly SignInManager<IUser> _signInManager;
        private readonly ILogger _logger;

        public AccountController
        (
            ApplicationUserManager userManager,
            ApplicationRoleManager roleManager,
            ApplicationAuthManager authManager,
            SignInManager<IUser> signInManager,
            ICacheManager cacheManager,
            ILogger<AccountController> logger,
            IMapper mapper
        )
        : base(authManager, logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _authManager = authManager;
            _signInManager = signInManager;
            _cacheProvider = cacheManager.CacheProvider;
            _logger = logger;
            _mapper = mapper;
        } 

        /// <summary>Authenticates the user.</summary>
        /// <param name="authenticateModel">The authenticate model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("users/authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateRequestApiModel authenticateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify client information
            if(!Guid.TryParse(authenticateModel.ClientId, out Guid clientId) || GuidHelper.IsNullOrEmpty(clientId))
            {
                return BadRequest($"Client '{clientId}' format is invalid."); 
            }

            string clientAuthResult = await _authManager.AuthenticateClientAsync(clientId, authenticateModel.ClientSecret);
            if(!string.IsNullOrEmpty(clientAuthResult))
            {
                return Unauthorized(clientAuthResult);
            }

            // Check user's status
            IUser user = await _userManager.FindByNameAsync(authenticateModel.UserName);
            if(user == null)
            {
                return Unauthorized($"Failed to log in - invalid username and/or password.");
            }
            if (user.IsDeleted)
            {
                return Unauthorized($"User [{user.UserName}] has been deleted.");
            }
            if (!user.IsApproved)
            {
                return Unauthorized($"User [{user.UserName}] is not approved.");
            }

            // Attempt to sign in
            // Note: CheckPasswordSignInAsync - this method doesn't perform 2fa check.
            // Note: PasswordSignInAsync - this method performs 2fa check, but also generates the ".AspNetCore.Identity.Application" cookie. Cookie creation cannot be disabled (SignInManager is heavily dependant on cookies - by design).
            // Note: isPersistent: false - no need to store browser cookies in Web API.
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, authenticateModel.Password, isPersistent: false, lockoutOnFailure: true); 
            return await AuthenticateAsync(signInResult, user, clientId);
        }

        /// <summary>Refreshes tokens (refresh and access tokens).</summary>
        /// <param name="refreshTokenModel">The refresh token model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("users/refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestApiModel refreshTokenModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify client information
            if (!Guid.TryParse(refreshTokenModel.ClientId, out Guid clientId) || GuidHelper.IsNullOrEmpty(clientId))
            {
                return BadRequest($"Client '{clientId}' format is invalid.");
            }

            string clientAuthResult = await _authManager.AuthenticateClientAsync(clientId, refreshTokenModel.ClientSecret);
            if (!string.IsNullOrEmpty(clientAuthResult))
            {
                return Unauthorized(clientAuthResult);
            }

            try
            {
                // Generate new tokens
                string accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                JwtAuthResult jwtResult = await _authManager.RefreshTokensAsync(refreshTokenModel.RefreshToken, accessToken, clientId);

                _logger.LogInformation("New access and refresh tokens are generated for the user.");

                RefreshTokenResponseApiModel authenticationResponse = new RefreshTokenResponseApiModel
                {
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken
                };

                return Ok(authenticationResponse);
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); 
            }
        }

        /// <summary>Deletes refresh tokens that have expired.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpDelete]
        [AuthorizationFilter(RoleHelper.Admin)]  
        [Route("users/expired-refresh-tokens")]
        public async Task<IActionResult> DeleteExpiredRefreshTokensAsync()
        {
            try
            {
                await _authManager.RemoveExpiredRefreshTokensAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Deletion of the expired refresh tokens has failed.", ex);

                return InternalServerError();
            }

            _logger.LogInformation("Expired refresh tokens have been successfully deleted.");

            return NoContent();
        }

        /// <summary>Retrieves all roles.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("roles")]
        [AuthorizationFilter(RoleHelper.Admin)]
        public async Task<IActionResult> GetRolesAsync()
        {
            IEnumerable<IRole> roles = await _cacheProvider.GetOrAddAsync
            (
                CacheParameters.Keys.AllRoles,
                _roleManager.GetRolesAsync,
                DateTimeOffset.MaxValue,
                CacheParameters.Groups.Identity
            );

            return Ok(_mapper.Map<IEnumerable<RoleGetApiModel>>(roles));
        }

        /// <summary>Registers a new user.</summary>
        /// <param name="registerUserModel">The register user model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("users/register")]
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
            IList<string> roles = new List<string>()
            {
                RoleHelper.Guest
            };
            IdentityResult roleResult = await _userManager.AddToRolesAsync(user, roles);

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

        /// <summary>Confirms the user's email.</summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("users/{id:guid}/confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromRoute] Guid id)
        {
            if (GuidHelper.IsNullOrEmpty(id))
            {
                return BadRequest("User Id is missing.");
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

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token.Base64ForUrlDecode());

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        /// <summary>Creates the specified user.</summary>
        /// <param name="createUserModel">The create user model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("users/create")]
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
        /// <param name="id">The user identifier.</param>
        /// <param name="userModel">The user model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPatch]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("users/{id:guid}")]
        public async Task<IActionResult> PatchUserAsync([FromRoute] Guid id, [FromBody] UserPatchApiModel userModel)
        {
            if (id == Guid.Empty)
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
            IUser user = await _userManager.FindUserByIdAsync(id, nameof(IUser.Roles));
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
        /// <param name="id">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("users/{id:guid}/unlock")]
        public async Task<IActionResult> UnlockUserAsync([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("User Id is missing.");

            IUser user = await _userManager.FindUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(-1));

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        /// <summary>Changes the user's password.</summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="changePasswordModel">The change password model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("users/{id:guid}/change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromRoute] Guid id, ChangePasswordPostApiModel changePasswordModel)
        {
            if (id == Guid.Empty)
                return BadRequest("User Id is missing.");

            IUser user = await _userManager.FindUserByIdAsync(id);
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


        /// <summary>Initiates the forgot password process for the user.</summary>
        /// <param name="email">The user's email.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("users/forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromQuery]string email)
        {
            if(string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is missing.");
            }

            IUser user = await _userManager.FindByEmailAsync(email);

            // User with verified email cannot be found, so return NotFound
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return NotFound();
            }

            // Get forgot password confirmation token
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            TokenResponseApiModel registerResponse = new TokenResponseApiModel
            {
                UserId = user.Id,
                ConfirmationToken = token.Base64ForUrlEncode()
            };

            return Ok(registerResponse);
        }

        /// <summary>Resets the user's password.</summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="forgotPasswordModel">The forgot password model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("users/{id:guid}/reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromRoute]Guid id, ResetPasswordPostApiModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string token = Request.Headers["Token"].First();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing.");
            }

            IUser user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, token.Base64ForUrlDecode(), forgotPasswordModel.Password);

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
        [Route("users")]
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
        /// <param name="id">The user identifier.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("users/{id:guid}")]
        public async Task<IActionResult> GetUserAsync([FromRoute] Guid id, [FromQuery] string[] includeProperties)
        {
            if (id == Guid.Empty)
                return BadRequest();

            IUser user = await _userManager.FindUserByIdAsync(id, ModelMapperHelper.GetPropertyMappings<UserGetApiModel, IUser>(_mapper, includeProperties));

            if (user != null)
                return Ok(_mapper.Map<UserGetApiModel>(user));

            return NotFound();
        }

        /// <summary>Assigns roles to the user.</summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="rolesToAssign">The roles to assign.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPut]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("users/{id:guid}/roles")]
        public async Task<IActionResult> AssignRolesToUserAsync([FromRoute] Guid id, string[] rolesToAssign)
        {
            if (rolesToAssign == null || rolesToAssign.Length == 0 || id == Guid.Empty)
            {
                return BadRequest();
            }

            // Find the user we want to assign roles to
            IUser user = await _userManager.FindByIdAsync(id.ToString());
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

            return Ok(new { userId = id, roles = rolesToAssign });
        }
    }
}