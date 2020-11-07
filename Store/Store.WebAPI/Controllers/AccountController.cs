using System;
using System.Web;
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
    [ApiExplorerSettings(IgnoreApi = false)]
    public class AccountController : ExtendedControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICacheProvider _cacheProvider;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly ApplicationAuthManager _authManager;
        private readonly SignInManager<IUser> _signInManager;
        private readonly ILogger _logger;

        public AccountController(
            ApplicationUserManager userManager,
            ApplicationRoleManager roleManager,
            ApplicationAuthManager authManager,
            SignInManager<IUser> signInManager,
            ICacheManager cacheManager,
            ILogger<AccountController> logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _authManager = authManager;
            _signInManager = signInManager;
            _cacheProvider = cacheManager.CacheProvider;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>Gets the user profile.</summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("users/{id:guid}/profile")]
        public async Task<IActionResult> GetUserProfileAsync([FromRoute] Guid id)
        {
            if (GuidHelper.IsNullOrEmpty(id))
            {
                return BadRequest("User Id is missing.");
            }

            IUser user = await _userManager.FindByIdAsync(id.ToString());
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

        /// <summary>Generates or retrieves authenticator key for the user.</summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("users/{id:guid}/authenticator-key")]
        public async Task<IActionResult> GetUserAuthenticatorKeyAsync([FromRoute] Guid id)
        {
            if (GuidHelper.IsNullOrEmpty(id))
            {
                return BadRequest("User Id is missing.");
            }

            IUser user = await _userManager.FindByIdAsync(id.ToString());

            string authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(authenticatorKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);

                _logger.LogInformation("A new authenticator key is generated.");
            }
            else
            {
                _logger.LogInformation("The existing authenticator key is retrieved from the database.");
            }

            AuthenticatorKeyGetApiModel authenticatorDetailsResponse =  new AuthenticatorKeyGetApiModel
            {
                SharedKey = authenticatorKey,
                AuthenticatorUri = GenerateQrCodeUri(user.Email, authenticatorKey)
            };

            return Ok(authenticatorDetailsResponse);
        }

        /// <summary>Verifies the authenticator code for the user. If successful, 2FA will be enabled.</summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="code">The authenticator code.</param>
        /// <returns>
        ///   Ten two factor recovery codes.
        /// </returns>
        [HttpPost]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("users/{id:guid}/verify-authenticator-code")]
        public async Task<IActionResult> VerifyUserAuthenticatorCodeAsync([FromRoute] Guid id, [FromQuery]string code)
        {
            if (GuidHelper.IsNullOrEmpty(id))
            {
                return BadRequest("User Id is missing.");
            }
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Verification Code is missing.");
            }

            IUser user = await _userManager.FindByIdAsync(id.ToString());
            bool isTwoFactorTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);

            if (isTwoFactorTokenValid)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);

                _logger.LogInformation("Two factor authentication is enabled for the user.");
            }
            else
            {
                return BadRequest("Verification Code is invalid.");
            }

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

                _logger.LogInformation("Ten two factor recovery codes are generatied for the user.");

                TwoFactoryRecoveryResponseApiModel response = new TwoFactoryRecoveryResponseApiModel
                {
                    RecoveryCodes = recoveryCodes.ToArray()
                };

                return Ok(response);
            }

            return NoContent();
        }

        /// <summary>
        /// Generates new two factor recovery codes for the user.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="number">The number.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("users/{id:guid}/generate-recovery-codes")]
        public async Task<IActionResult> GenerateNewTwoFactorRecoveryCodesAsync([FromRoute] Guid id, [FromQuery]int number)
        {
            if (GuidHelper.IsNullOrEmpty(id))
            {
                return BadRequest("User Id is missing.");
            }

            IUser user = await _userManager.FindByIdAsync(id.ToString());

            if (await _userManager.CountRecoveryCodesAsync(user) != 0)
            {
                return BadRequest("Cannot generate new recovery codes as old ones have not been used.");
            }

            IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, number);

            TwoFactoryRecoveryResponseApiModel response = new TwoFactoryRecoveryResponseApiModel
            {
                RecoveryCodes = recoveryCodes.ToArray()
            };

            return Ok(response);
        }

        /// <summary>Disables the two factor authentication.</summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("users/{id:guid}/disable-two-factor")]
        public async Task<IActionResult> DisableTwoFactorAsync([FromRoute] Guid id)
        {
            if (GuidHelper.IsNullOrEmpty(id))
            {
                return BadRequest("User Id is missing.");
            }

            IUser user = await _userManager.FindByIdAsync(id.ToString());

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return BadRequest("Cannot disable two factor authentication as it's not currently enabled.");
            }

            IdentityResult result = await _userManager.SetTwoFactorEnabledAsync(user, false);

            _logger.LogInformation("Two factor authentication is disabled for the user.");

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        /// <summary>Authenticates the user using the two factor authentication code.</summary>
        /// <param name="authenticateModel">The authenticate model.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        [Route("users/two-factor-authenticate")]
        public async Task<IActionResult> TwoFactorAuthenticateAsync(AuthenticateTwoFactorRequestApiModel authenticateModel)
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

            IUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if(user == null)
            {
                return BadRequest("Two-factor authentication action is not supported for the user.");
            }

            SignInResult signInResult;
            if (!authenticateModel.UseRecoveryCode)
            {
                //Note: isPersistent, rememberClient: false - no need to store browser cookies in Web API.
                signInResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticateModel.Code, false, false);
            }
            else
            {
                signInResult = await _signInManager.TwoFactorRecoveryCodeSignInAsync(authenticateModel.Code);
            }

            IActionResult response = await AuthenticateAsync(signInResult, user, clientId);

            return Ok(response);
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

            ClientAuthResult clientAuthResult = await _authManager.ValidateClientAuthenticationAsync(clientId, authenticateModel.ClientSecret);
            if(!clientAuthResult.Succeeded)
            {
                return Unauthorized(clientAuthResult.ErrorMessage);
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
            IActionResult response = await AuthenticateAsync(signInResult, user, clientId);

            return response;
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

            ClientAuthResult clientAuthResult = await _authManager.ValidateClientAuthenticationAsync(clientId, refreshTokenModel.ClientSecret);
            if (!clientAuthResult.Succeeded)
            {
                return Unauthorized(clientAuthResult.ErrorMessage);
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

        /// <summary>Deletes the expired refresh tokens.</summary>
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
            IEnumerable<IRole> roles = await GetRolesFromCache();

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

        //[HttpPost]
        //[AllowAnonymous]
        //public IActionResult ExternalLogin(string provider, string returnUrl = null)
        //{
        //    // Request a redirect to the external login provider.
        //    var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        //    return Challenge(properties, provider);
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        //{
        //    if (remoteError != null)
        //    {
        //        ErrorMessage = $"Error from external provider: {remoteError}";

        //        return RedirectToAction(nameof(Login));
        //    }
        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        return RedirectToAction(nameof(Login));
        //    }

        //    // Sign in the user with this external login provider if the user already has a login.
        //    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        //    if (result.Succeeded)
        //    {
        //        _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);

        //        return RedirectToLocal(returnUrl);
        //    }
        //    if (result.IsLockedOut)
        //    {
        //        return RedirectToAction(nameof(Lockout));
        //    }
        //    else
        //    {
        //        // If the user does not have an account, then ask the user to create an account.
        //        ViewData["ReturnUrl"] = returnUrl;
        //        ViewData["LoginProvider"] = info.LoginProvider;
        //        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        //        return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
        //    }
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await _signInManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            throw new ApplicationException("Error loading external login information during confirmation.");
        //        }

        //        var user = new User { UserName = model.Email, Email = model.Email };
        //        var result = await _userManager.CreateAsync(user);

        //        if (result.Succeeded)
        //        {
        //            result = await _userManager.AddLoginAsync(user, info);
        //            if (result.Succeeded)
        //            {
        //                await _signInManager.SignInAsync(user, isPersistent: false);
        //                _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewData["ReturnUrl"] = returnUrl;

        //    return View(nameof(ExternalLogin), model);
        //

        #region Helpers

        private Task<IEnumerable<IRole>> GetRolesFromCache()
        {
            return _cacheProvider.GetOrAddAsync
            (
                CacheParameters.Keys.AllRoles,
                _roleManager.GetRolesAsync,
                DateTimeOffset.MaxValue,
                CacheParameters.Groups.Identity
            );
        }

        private IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            return BadRequest(result.Errors);
        }

        private string GenerateQrCodeUri(string email, string authenticatorKey)
        {
            const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

            return string.Format
            (
                AuthenticatorUriFormat,
                HttpUtility.UrlPathEncode("ASP.NET Core Identity"),
                HttpUtility.UrlPathEncode(email),
                authenticatorKey
            );
        }

        private async Task<IActionResult> AuthenticateAsync(SignInResult signInResult, IUser user, Guid clientId)
        {
            AuthenticateResponseApiModel authenticationResponse = new AuthenticateResponseApiModel
            {
                UserId = user.Id,
                RequiresTwoFactor = signInResult.RequiresTwoFactor
            };

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    return Unauthorized($"User [{user.UserName}] has been locked out.");
                }
                if (signInResult.IsNotAllowed)
                {
                    return Unauthorized($"User [{user.UserName}] is not allowed to log in.");
                }
                if (signInResult.RequiresTwoFactor)
                {
                    return Ok(authenticationResponse);
                }

                return Unauthorized($"Failed to log in [{user.UserName}].");
            }

            _logger.LogInformation($"User [{user.UserName}] has logged in the system.");

            JwtAuthResult jwtResult = await _authManager.GenerateTokensAsync(user.Id, clientId);

            authenticationResponse.Roles = jwtResult.Roles.ToArray();
            authenticationResponse.AccessToken = jwtResult.AccessToken;
            authenticationResponse.RefreshToken = jwtResult.RefreshToken;

            if (signInResult.Succeeded)
            {
                // Need to delete "identity" cookie for the authorized user - created by SignInManager 
                Response.Cookies.Delete(".AspNetCore.Identity.Application");
                Response.Headers.Remove("Set-Cookie");
            }

            return Ok(authenticationResponse);
        }

        #endregion
    }
}