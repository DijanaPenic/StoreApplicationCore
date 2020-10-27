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
using Store.Cache.Common;
using Store.WebAPI.Models;
using Store.WebAPI.Identity;
using Store.WebAPI.Constants;
using Store.Common.Helpers;
using Store.Web.Controllers;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Store.Models.Identity;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ExtendedControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICacheProvider _cacheProvider;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly ApplicationAuthManager _authManager;
        private readonly SignInManager<IUser> _signInManager;
        // TODO - resolve email sender
        //private readonly IEmailSender _emailSender;


        private readonly ILogger _logger;

        public AccountController(
            ApplicationUserManager userManager,
            ApplicationRoleManager roleManager,
            ApplicationAuthManager authManager,
            SignInManager<IUser> signInManager,
            ICacheManager cacheManager,
            //IEmailSender emailSender,
            ILogger<AccountController> logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _authManager = authManager;
            _signInManager = signInManager;
            _cacheProvider = cacheManager.CacheProvider;
            //_emailSender = emailSender;
            _logger = logger;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("users/authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateRequestApiModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Verify client information
            if(!Guid.TryParse(model.ClientId, out Guid clientId))
            {
                return BadRequest($"Client '{clientId}' format is invalid."); 
            }

            ClientAuthResult clientAuthResult = await _authManager.ValidateClientAuthenticationAsync(clientId, model.ClientSecret);

            if(!clientAuthResult.Succeeded)
            {
                return Unauthorized(clientAuthResult.ErrorMessage);
            }

            // Check user's status
            IUser user = await _userManager.FindByNameAsync(model.UserName);
            if(user == null)
            {
                return Unauthorized($"Failed to log in - invalid username and/or password.");
            }
            if (user.IsDeleted)
            {
                return Unauthorized($"User [{model.UserName}] has been deleted.");
            }

            // Attempt to sign in the specificied username and password
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: true, lockoutOnFailure: true);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    return Unauthorized($"User [{model.UserName}] has been locked out.");
                }
                if (signInResult.IsNotAllowed)
                {
                    return Unauthorized($"User [{model.UserName}] is not allowed to log in.");
                }
                if (signInResult.RequiresTwoFactor)
                {
                    return Unauthorized($"User [{model.UserName}] requires two-factor authentication.");
                }

                return Unauthorized($"Failed to log in - invalid username and/or password.");
            }

            _logger.LogInformation($"User [{model.UserName}] has logged in the system.");

            JwtAuthResult jwtResult = await _authManager.GenerateTokensAsync(model.UserName, clientId);

            AuthenticateResponseApiModel authenticationResponse = new AuthenticateResponseApiModel
            {
                UserName = model.UserName,
                Roles = jwtResult.Roles.ToArray(),
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.Value
            };

            return Ok(authenticationResponse);
        }

        [Route("roles")]
        public async Task<IActionResult> GetRolesAsync()
        {
            IEnumerable<IRole> roles = await GetRolesFromCache();

            return Ok(_mapper.Map<IEnumerable<RoleGetApiModel>>(roles));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("users/create")]
        public async Task<IActionResult> Create(RegisterPostApiModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            IUser user = _mapper.Map<IUser>(model);
            IdentityResult userResult = await _userManager.CreateAsync(user, model.Password);

            if (!userResult.Succeeded) return GetErrorResult(userResult);

            _logger.LogInformation("User created a new account with password.");

            IdentityResult roleResult = await _userManager.AddToRolesAsync(user, model.Roles);

            if (!roleResult.Succeeded) return GetErrorResult(roleResult);

            _logger.LogInformation("User assigned to roles.");

            return Ok();
        }

        [HttpPatch]
        [Route("users/{id:guid}")]
        public async Task<IActionResult> PatchUserAsync([FromRoute] Guid id, [FromBody] UserPatchApiModel model)
        {
            if (id == Guid.Empty)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isRoleSelectionValid = await _roleManager.IsValidRoleSelectionAsync(model.Roles);
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

            _mapper.Map(model, user);
            IdentityResult userResult = await _userManager.UpdateAsync(user);

            if (!userResult.Succeeded) return GetErrorResult(userResult);

            // Remove user from roles that are not listed in model.roles
            IEnumerable<string> rolesToRemove = user.Roles.Where(r => !model.Roles.Contains(r.Name)).Select(r => r.Name);
            IdentityResult removeRolesResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (!removeRolesResult.Succeeded) return GetErrorResult(removeRolesResult);

            // Assign user to roles
            if (model.Roles != null)
            {
                IEnumerable<string> rolesToAdd = model.Roles.Except(user.Roles.Select(r => r.Name));
                IdentityResult addRolesResult = await _userManager.AddToRolesAsync(user, rolesToAdd);

                if (!addRolesResult.Succeeded) return GetErrorResult(addRolesResult);
            }

            return Ok();
        }

        [HttpPost]
        [Route("users/{id:guid}/unlock")]
        public async Task<IActionResult> UnlockUserAsync([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            IUser user = await _userManager.FindUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(-1));

            if (!result.Succeeded) return GetErrorResult(result);

            return Ok();
        }

        [HttpPost]
        [Route("users/{id:guid}/change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromRoute] Guid id, ChangePasswordPostApiModel model)
        {
            if (id == Guid.Empty)
                return BadRequest();

            IUser user = await _userManager.FindUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        [HttpGet]
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
                return Ok(_mapper.Map<PagedResponse<UserGetApiModel>>(users)); 
            }

            return NoContent();
        }

        [HttpGet]
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

        [Route("users/{id:guid}/roles")]
        [HttpPut]
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
            if (!removeResult.Succeeded)
            {
                BadRequest("Failed to remove user roles.");
            }

            // Assign user to the new roles
            IdentityResult addResult = await _userManager.AddToRolesAsync(user, rolesToAssign);
            if (addResult.Succeeded)
            { 
                return Ok(new { userId = id, roles = rolesToAssign });
            }

            return BadRequest("Failed to add user roles.");
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> Login(string returnUrl = null)
        //{
        //    // Clear the existing external cookie to ensure a clean login process
        //    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        //    ViewData["ReturnUrl"] = returnUrl;

        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        //{
        //    ViewData["ReturnUrl"] = returnUrl;

        //    if (ModelState.IsValid)
        //    {
        //        // This doesn't count login failures towards account lockout
        //        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        //        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        //        if (result.Succeeded)
        //        {
        //            _logger.LogInformation("User logged in.");

        //            return RedirectToLocal(returnUrl);
        //        }
        //        if (result.RequiresTwoFactor)
        //        {
        //            return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
        //        }
        //        if (result.IsLockedOut)
        //        {
        //            _logger.LogWarning("User account locked out.");

        //            return RedirectToAction(nameof(Lockout));
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

        //            return View(model);
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        //{
        //    // Ensure the user has gone through the username & password screen first
        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load two-factor authentication user.");
        //    }

        //    var model = new LoginWith2faViewModel { RememberMe = rememberMe };
        //    ViewData["ReturnUrl"] = returnUrl;

        //    return View(model);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    }

        //    var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        //    var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

        //    if (result.Succeeded)
        //    {
        //        _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);

        //        return RedirectToLocal(returnUrl);
        //    }
        //    else if (result.IsLockedOut)
        //    {
        //        _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);

        //        return RedirectToAction(nameof(Lockout));
        //    }
        //    else
        //    {
        //        _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
        //        ModelState.AddModelError(string.Empty, "Invalid authenticator code.");

        //        return View();
        //    }
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        //{
        //    // Ensure the user has gone through the username & password screen first
        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load two-factor authentication user.");
        //    }

        //    ViewData["ReturnUrl"] = returnUrl;

        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load two-factor authentication user.");
        //    }

        //    var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

        //    var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        //    if (result.Succeeded)
        //    {
        //        _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);

        //        return RedirectToLocal(returnUrl);
        //    }
        //    if (result.IsLockedOut)
        //    {
        //        _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);

        //        return RedirectToAction(nameof(Lockout));
        //    }
        //    else
        //    {
        //        _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
        //        ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");

        //        return View();
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();
        //    _logger.LogInformation("User logged out.");

        //    return RedirectToAction(nameof(HomeController.Index), "Home");
        //}

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
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return RedirectToAction(nameof(HomeController.Index), "Home");
        //    }

        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load user with ID '{userId}'.");
        //    }

        //    var result = await _userManager.ConfirmEmailAsync(user, code);

        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(model.Email);
        //        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        //        }

        //        // For more information on how to enable account confirmation and password reset please
        //        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        //        //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        //var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
        //        //await _emailSender.SendEmailAsync(model.Email, "Reset Password",
        //        //   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

        //        return RedirectToAction(nameof(ForgotPasswordConfirmation));
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ResetPassword(string code = null)
        //{
        //    if (code == null)
        //    {
        //        throw new ApplicationException("A code must be supplied for password reset.");
        //    }
        //    var model = new ResetPasswordViewModel { Code = code };

        //    return View(model);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction(nameof(ResetPasswordConfirmation));
        //    }

        //    var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction(nameof(ResetPasswordConfirmation));
        //    }
        //    AddErrors(result);

        //    return View();
        //}

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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            return BadRequest(result.Errors);
        }

        #endregion
    }
}