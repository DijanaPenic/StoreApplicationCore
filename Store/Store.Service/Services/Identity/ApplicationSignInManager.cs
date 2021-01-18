using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using Store.Service.Constants;
using Store.Model.Common.Models.Identity;

namespace Store.Services.Identity
{
    public class ApplicationSignInManager
    {
        private const string LoginProviderKey = "LoginProvider";
        private const string XsrfKey = "XsrfId";

        public ApplicationSignInManager(ApplicationUserManager userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<IUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<ApplicationSignInManager> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<IUser> confirmation)
        {
            UserManager = userManager;
            _contextAccessor = contextAccessor;
            ClaimsFactory = claimsFactory;
            Options = optionsAccessor?.Value ?? new IdentityOptions();
            Logger = logger;
            _schemes = schemes;
            _confirmation = confirmation;
        }

        private HttpContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAuthenticationSchemeProvider _schemes;
        private readonly IUserConfirmation<IUser> _confirmation;

        public ILogger Logger { get; set; }

        public ApplicationUserManager UserManager { get; set; }

        public IUserClaimsPrincipalFactory<IUser> ClaimsFactory { get; set; }

        public IdentityOptions Options { get; set; }

        public HttpContext Context
        {
            get
            {
                HttpContext context = _context ?? _contextAccessor?.HttpContext;
                if (context == null)
                {
                    throw new InvalidOperationException("HttpContext must not be null.");
                }
                return context;
            }
            set
            {
                _context = value;
            }
        }

        public virtual async Task<ClaimsPrincipal> CreateUserPrincipalAsync(IUser user) => await ClaimsFactory.CreateAsync(user);

        /// <summary>
        /// Returns a flag indicating whether the specified user can sign in.
        /// </summary>
        /// <param name="user">The user whose sign-in status should be returned.</param>
        /// <returns>
        /// The task object representing the asynchronous operation, containing a flag that is true
        /// if the specified user can sign-in, otherwise false.
        /// </returns>
        public virtual async Task<bool> CanSignInAsync(IUser user)
        {
            if (Options.SignIn.RequireConfirmedEmail && !(await UserManager.IsEmailConfirmedAsync(user)))
            {
                Logger.LogWarning(0, "User {userId} cannot sign in without a confirmed email.", await UserManager.GetUserIdAsync(user));

                return false;
            }
            if (Options.SignIn.RequireConfirmedPhoneNumber && !(await UserManager.IsPhoneNumberConfirmedAsync(user)))
            {
                Logger.LogWarning(1, "User {userId} cannot sign in without a confirmed phone number.", await UserManager.GetUserIdAsync(user));

                return false;
            }
            if (Options.SignIn.RequireConfirmedAccount && !(await _confirmation.IsConfirmedAsync(UserManager, user)))
            {
                Logger.LogWarning(4, "User {userId} cannot sign in without a confirmed account.", await UserManager.GetUserIdAsync(user));

                return false;
            }

            return true;
        }

        /// <summary>
        /// Signs in the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to sign-in.</param>
        /// <param name="authenticationProperties">Properties applied to the login and authentication cookie.</param>
        /// <param name="authenticationMethod">Name of the method used to authenticate the user.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SignInAsync(IUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            List<Claim> additionalClaims = new List<Claim>();

            if (authenticationMethod != null)
            {
                additionalClaims.Add(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            }

            return Task.CompletedTask;
        }


        /// <summary>
        /// Signs the current user out of the application.
        /// </summary>
        public virtual async Task SignOutAsync()
        {
            await Context.SignOutAsync(IdentityConstants.ExternalScheme);
            await Context.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);
            await Context.SignOutAsync(ApplicationIdentityConstants.AccountVerificationScheme);
        }

        /// <summary>
        /// Validates the security stamp for the specified <paramref name="principal"/> against
        /// the persisted stamp for the current user, as an asynchronous operation.
        /// </summary>
        /// <param name="principal">The principal whose stamp should be validated.</param>
        /// <returns>The task object representing the asynchronous operation. The task will contain the <typeparamref name="IUser"/>
        /// if the stamp matches the persisted value, otherwise it will return false.</returns>
        public virtual async Task<IUser> ValidateSecurityStampAsync(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return null;
            }

            IUser user = await UserManager.GetUserAsync(principal);
            if (await ValidateSecurityStampAsync(user, principal.FindFirstValue(Options.ClaimsIdentity.SecurityStampClaimType)))
            {
                return user;
            }
            Logger.LogDebug(4, "Failed to validate a security stamp.");

            return null;
        }

        /// <summary>
        /// Validates the security stamp for the specified <paramref name="principal"/> from one of
        /// the two factor principals (remember client or user id) against
        /// the persisted stamp for the current user, as an asynchronous operation.
        /// </summary>
        /// <param name="principal">The principal whose stamp should be validated.</param>
        /// <returns>The task object representing the asynchronous operation. The task will contain the <typeparamref name="IUser"/>
        /// if the stamp matches the persisted value, otherwise it will return false.</returns>
        public virtual async Task<IUser> ValidateTwoFactorSecurityStampAsync(ClaimsPrincipal principal)
        {
            if (principal == null || principal.Identity?.Name == null)
            {
                return null;
            }

            IUser user = await UserManager.FindByIdAsync(principal.Identity.Name);
            if (await ValidateSecurityStampAsync(user, principal.FindFirstValue(Options.ClaimsIdentity.SecurityStampClaimType)))
            {
                return user;
            }
            Logger.LogDebug(5, "Failed to validate a security stamp.");

            return null;
        }

        /// <summary>
        /// Validates the security stamp for the specified <paramref name="user"/>. Will always return false
        /// if the userManager does not support security stamps.
        /// </summary>
        /// <param name="user">The user whose stamp should be validated.</param>
        /// <param name="securityStamp">The expected security stamp value.</param>
        /// <returns>True if the stamp matches the persisted value, otherwise it will return false.</returns>
        public virtual async Task<bool> ValidateSecurityStampAsync(IUser user, string securityStamp) => user != null &&
            // Only validate the security stamp if the store supports it
            (!UserManager.SupportsUserSecurityStamp || securityStamp == await UserManager.GetSecurityStampAsync(user));

        /// <summary>
        /// Attempts to sign in the specified <paramref name="user"/> and <paramref name="password"/> combination
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="user">The user to sign in.</param>
        /// <param name="password">The password to attempt to sign in with.</param>
        /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign in fails.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
        /// for the sign-in attempt.</returns>
        public virtual async Task<SignInResult> PasswordSignInAsync(Guid clientId, IUser user, string password, bool lockoutOnFailure)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            SignInResult passwordAttempt = await CheckPasswordSignInAsync(clientId, user, password, lockoutOnFailure);

            return passwordAttempt.Succeeded ? await SignInOrTwoFactorAsync(clientId, user) : passwordAttempt;
        }

        /// <summary>
        /// Attempts to sign in the specified <paramref name="userName"/> and <paramref name="password"/> combination
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="userName">The user name to sign in.</param>
        /// <param name="password">The password to attempt to sign in with.</param>
        /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign in fails.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
        /// for the sign-in attempt.</returns>
        public virtual async Task<SignInResult> PasswordSignInAsync(Guid clientId, string userName, string password,  bool lockoutOnFailure)
        {
            IUser user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await PasswordSignInAsync(clientId, user, password, lockoutOnFailure);
        }

        /// <summary>
        /// Attempts a password sign in for a user.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="user">The user to sign in.</param>
        /// <param name="password">The password to attempt to sign in with.</param>
        /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign in fails.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
        /// for the sign-in attempt.</returns>
        /// <returns></returns>
        public virtual async Task<SignInResult> CheckPasswordSignInAsync(Guid clientId, IUser user, string password, bool lockoutOnFailure)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            SignInResult error = await PreSignInCheck(clientId, user);
            if (error != null)
            {
                return error;
            }

            if (await UserManager.CheckPasswordAsync(user, password))
            {
                bool alwaysLockout = AppContext.TryGetSwitch("Microsoft.AspNetCore.Identity.CheckPasswordSignInAlwaysResetLockoutOnSuccess", out bool enabled) && enabled;
                // Only reset the lockout when TFA is not enabled when not in quirks mode
                if (alwaysLockout || !await IsTfaEnabled(user))
                {
                    await ResetLockout(user);
                }

                return SignInResult.Success;
            }
            Logger.LogWarning(2, "User {userId} failed to provide the correct password.", await UserManager.GetUserIdAsync(user));

            if (UserManager.SupportsUserLockout && lockoutOnFailure)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user);
                if (await UserManager.IsLockedOutAsync(user))
                {
                    return await LockedOut(user);
                }
            }

            return SignInResult.Failed;
        }

        /// <summary>
        /// Returns a flag indicating if the current client browser has been remembered by two factor authentication
        /// for the user attempting to login, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user attempting to login.</param>
        /// <returns>
        /// The task object representing the asynchronous operation containing true if the browser has been remembered
        /// for the current user.
        /// </returns>
        public virtual async Task<bool> IsTwoFactorClientRememberedAsync(IUser user)
        {
            string userId = await UserManager.GetUserIdAsync(user);
            AuthenticateResult result = await Context.AuthenticateAsync(IdentityConstants.TwoFactorRememberMeScheme);

            return (result?.Principal != null && result.Principal.FindFirstValue(ClaimTypes.Name) == userId);
        }

        /// <summary>
        /// Sets a flag on the browser to indicate the user has selected "Remember this browser" for two factor authentication purposes,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user who choose "remember this browser".</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual async Task RememberTwoFactorClientAsync(IUser user)
        {
            ClaimsPrincipal principal = await StoreRememberClient(user);

            await Context.SignInAsync(IdentityConstants.TwoFactorRememberMeScheme, principal, new AuthenticationProperties { IsPersistent = true });
        }

        /// <summary>
        /// Clears the "Remember this browser flag" from the current browser, as an asynchronous operation.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task ForgetTwoFactorClientAsync()
        {
            return Context.SignOutAsync(IdentityConstants.TwoFactorRememberMeScheme);
        }

        /// <summary>
        /// Signs in the user without two factor authentication using a two factor recovery code.
        /// </summary>
        /// <param name="recoveryCode">The two factor recovery code.</param>
        /// <returns></returns>
        public virtual async Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
        {
            TwoFactorAuthenticationInfo twoFactorInfo = await GetTwoFactorInfoAsync();
            if (twoFactorInfo == null || twoFactorInfo.UserId == null)
            {
                return SignInResult.Failed;
            }

            IUser user = await UserManager.FindByIdAsync(twoFactorInfo.UserId);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            IdentityResult result = await UserManager.RedeemTwoFactorRecoveryCodeAsync(user, recoveryCode);
            if (result.Succeeded)
            {
                await DoTwoFactorSignInAsync(user, twoFactorInfo, rememberClient: false);

                return SignInResult.Success;
            }

            // We don't protect against brute force attacks since codes are expected to be random.
            return SignInResult.Failed;
        }

        private async Task DoTwoFactorSignInAsync(IUser user, TwoFactorAuthenticationInfo twoFactorInfo,  bool rememberClient)
        {
            // When token is verified correctly, clear the access failed count used for lockout
            await ResetLockout(user);

            List<Claim> claims = new List<Claim>
            {
                new Claim("amr", "mfa")
            };

            // Cleanup external cookie
            if (twoFactorInfo.LoginProvider != null)
            {
                claims.Add(new Claim(ClaimTypes.AuthenticationMethod, twoFactorInfo.LoginProvider));
                await Context.SignOutAsync(IdentityConstants.ExternalScheme);
            }

            // Cleanup two factor user id cookie
            await Context.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);
            if (rememberClient)
            {
                await RememberTwoFactorClientAsync(user);
            }
        }

        /// <summary>
        /// Validates the sign in code from an authenticator app and creates and signs in the user, as an asynchronous operation.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="code">The two factor authentication code to validate.</param>
        /// <param name="rememberClient">Flag indicating whether the current browser should be remember, suppressing all further 
        /// two factor authentication prompts.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
        /// for the sign-in attempt.</returns>
        public virtual async Task<SignInResult> TwoFactorAuthenticatorSignInAsync(Guid clientId, string code, bool rememberClient)
        {
            TwoFactorAuthenticationInfo twoFactorInfo = await GetTwoFactorInfoAsync();
            if (twoFactorInfo == null || twoFactorInfo.UserId == null)
            {
                return SignInResult.Failed;
            }

            IUser user = await UserManager.FindByIdAsync(twoFactorInfo.UserId);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            SignInResult error = await PreSignInCheck(clientId, user);
            if (error != null)
            {
                return error;
            }

            if (await UserManager.VerifyTwoFactorTokenAsync(user, Options.Tokens.AuthenticatorTokenProvider, code))
            {
                await DoTwoFactorSignInAsync(user, twoFactorInfo, rememberClient);

                return SignInResult.Success;
            }

            // If the token is incorrect, record the failure which also may cause the user to be locked out
            await UserManager.AccessFailedAsync(user);

            return SignInResult.Failed;
        }

        /// <summary>
        /// Validates the two factor sign in code and creates and signs in the user, as an asynchronous operation.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="provider">The two factor authentication provider to validate the code against.</param>
        /// <param name="code">The two factor authentication code to validate.</param>
        /// <param name="rememberClient">Flag indicating whether the current browser should be remember, suppressing all further 
        /// two factor authentication prompts.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
        /// for the sign-in attempt.</returns>
        public virtual async Task<SignInResult> TwoFactorSignInAsync(Guid clientId, string provider, string code, bool rememberClient)
        {
            TwoFactorAuthenticationInfo twoFactorInfo = await GetTwoFactorInfoAsync();
            if (twoFactorInfo == null || twoFactorInfo.UserId == null)
            {
                return SignInResult.Failed;
            }

            IUser user = await UserManager.FindByIdAsync(twoFactorInfo.UserId);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            SignInResult error = await PreSignInCheck(clientId, user);
            if (error != null)
            {
                return error;
            }

            if (await UserManager.VerifyTwoFactorTokenAsync(user, provider, code))
            {
                await DoTwoFactorSignInAsync(user, twoFactorInfo, rememberClient);

                return SignInResult.Success;
            }

            // If the token is incorrect, record the failure which also may cause the user to be locked out
            await UserManager.AccessFailedAsync(user);

            return SignInResult.Failed;
        }

        /// <summary>
        /// Gets the <typeparamref name="IUser"/> for the current two factor authentication login, as an asynchronous operation.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation containing the <typeparamref name="IUser"/>
        /// for the sign-in attempt.</returns>
        public virtual async Task<IUser> GetTwoFactorAuthenticationUserAsync()
        {
            TwoFactorAuthenticationInfo info = await GetTwoFactorInfoAsync();
            if (info == null)
            {
                return null;
            }

            return await UserManager.FindByIdAsync(info.UserId);
        }

        /// <summary>
        /// Signs in a user via a previously registered third party login, as an asynchronous operation.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="loginProvider">The login provider to use.</param>
        /// <param name="providerKey">The unique provider identifier for the user.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
        /// for the sign-in attempt.</returns>
        public virtual Task<SignInResult> ExternalLoginSignInAsync(Guid clientId, string loginProvider, string providerKey) => ExternalLoginSignInAsync(clientId, loginProvider, providerKey, bypassTwoFactor: false);

        /// <summary>
        /// Signs in a user via a previously registered third party login, as an asynchronous operation.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="loginProvider">The login provider to use.</param>
        /// <param name="providerKey">The unique provider identifier for the user.</param>
        /// <param name="bypassTwoFactor">Flag indicating whether to bypass two factor authentication.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
        /// for the sign-in attempt.</returns>
        public virtual async Task<SignInResult> ExternalLoginSignInAsync(Guid clientId, string loginProvider, string providerKey, bool bypassTwoFactor)
        {
            IUser user = await UserManager.FindByLoginAsync(loginProvider, providerKey);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            SignInResult error = await PreSignInCheck(clientId, user);
            if (error != null)
            {
                return error;
            }

            return await SignInOrTwoFactorAsync(clientId, user, loginProvider, bypassTwoFactor);
        }

        /// <summary>
        /// Gets a collection of <see cref="AuthenticationScheme"/>s for the known external login providers.		
        /// </summary>		
        /// <returns>A collection of <see cref="AuthenticationScheme"/>s for the known external login providers.</returns>		
        public virtual async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            IEnumerable<AuthenticationScheme> schemes = await _schemes.GetAllSchemesAsync();

            return schemes.Where(s => !string.IsNullOrEmpty(s.DisplayName));
        }

        /// <summary>
        /// Gets the external login information for the current login, as an asynchronous operation.
        /// </summary>
        /// <param name="expectedXsrf">Flag indication whether a Cross Site Request Forgery token was expected in the current request.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="ExternalLoginInfo"/>
        /// for the sign-in attempt.</returns>
        public virtual async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
        {
            AuthenticateResult auth = await Context.AuthenticateAsync(IdentityConstants.ExternalScheme);
            IDictionary<string, string> items = auth?.Properties?.Items;

            if (auth?.Principal == null || items == null || !items.ContainsKey(LoginProviderKey))
            {
                return null;
            }

            if (expectedXsrf != null)
            {
                if (!items.ContainsKey(XsrfKey))
                {
                    return null;
                }

                string userId = items[XsrfKey] as string;
                if (userId != expectedXsrf)
                {
                    return null;
                }
            }

            string providerKey = auth.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (providerKey == null || items[LoginProviderKey] is not string provider)
            {
                return null;
            }

            string providerDisplayName = (await GetExternalAuthenticationSchemesAsync()).FirstOrDefault(p => p.Name == provider)?.DisplayName ?? provider;

            return new ExternalLoginInfo(auth.Principal, provider, providerKey, providerDisplayName)
            {
                AuthenticationTokens = auth.Properties.GetTokens()
            };
        }

        /// <summary>
        /// Stores any authentication tokens found in the external authentication cookie into the associated user.
        /// </summary>
        /// <param name="externalLogin">The information from the external login provider.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the operation.</returns>
        public virtual async Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo externalLogin)
        {
            if (externalLogin == null)
            {
                throw new ArgumentNullException(nameof(externalLogin));
            }

            if (externalLogin.AuthenticationTokens != null && externalLogin.AuthenticationTokens.Any())
            {
                IUser user = await UserManager.FindByLoginAsync(externalLogin.LoginProvider, externalLogin.ProviderKey);
                if (user == null)
                {
                    return IdentityResult.Failed();
                }

                foreach (AuthenticationToken token in externalLogin.AuthenticationTokens)
                {
                    IdentityResult result = await UserManager.SetAuthenticationTokenAsync(user, externalLogin.LoginProvider, token.Name, token.Value);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Configures the redirect URL and user identifier for the specified external login <paramref name="provider"/>.
        /// </summary>
        /// <param name="provider">The provider to configure.</param>
        /// <param name="redirectUrl">The external login URL users should be redirected to during the login flow.</param>
        /// <param name="userId">The current user's identifier, which will be used to provide CSRF protection.</param>
        /// <returns>A configured <see cref="AuthenticationProperties"/>.</returns>
        public virtual AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null)
        {
            AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items[LoginProviderKey] = provider;

            if (userId != null)
            {
                properties.Items[XsrfKey] = userId;
            }

            return properties;
        }

        /// <summary>
        /// Creates a claims principal for the specified 2fa information.
        /// </summary>
        /// <param name="userId">The user whose is logging in via 2fa.</param>
        /// <param name="loginProvider">The 2fa provider.</param>
        /// <returns>A <see cref="ClaimsPrincipal"/> containing the user 2fa information.</returns>
        internal static ClaimsPrincipal StoreTwoFactorInfo(string clientId, string userId, string loginProvider)
        {
            ClaimsIdentity identity = new ClaimsIdentity(IdentityConstants.TwoFactorUserIdScheme);

            identity.AddClaim(new Claim(ClaimTypes.Name, userId));
            identity.AddClaim(new Claim(ApplicationClaimTypes.ClientIdentifier, clientId));

            if (loginProvider != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, loginProvider));
            }

            return new ClaimsPrincipal(identity);
        }

        internal async Task<ClaimsPrincipal> StoreRememberClient(IUser user)
        {
            string userId = await UserManager.GetUserIdAsync(user);
            ClaimsIdentity rememberBrowserIdentity = new ClaimsIdentity(IdentityConstants.TwoFactorRememberMeScheme);
            rememberBrowserIdentity.AddClaim(new Claim(ClaimTypes.Name, userId));

            if (UserManager.SupportsUserSecurityStamp)
            {
                string stamp = await UserManager.GetSecurityStampAsync(user);
                rememberBrowserIdentity.AddClaim(new Claim(Options.ClaimsIdentity.SecurityStampClaimType, stamp));
            }

            return new ClaimsPrincipal(rememberBrowserIdentity);
        }

        private async Task<bool> IsTfaEnabled(IUser user) => UserManager.SupportsUserTwoFactor && await UserManager.GetTwoFactorEnabledAsync(user) && (await UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;

        /// <summary>
        /// Signs in the specified <paramref name="user"/> if <paramref name="bypassTwoFactor"/> is set to false.
        /// Otherwise stores the <paramref name="user"/> for use after a two factor check.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="user"></param>
        /// <param name="loginProvider">The login provider to use. Default is null</param>
        /// <param name="bypassTwoFactor">Flag indicating whether to bypass two factor authentication. Default is false</param>
        /// <returns>Returns a <see cref="SignInResult"/></returns>
        protected virtual async Task<SignInResult> SignInOrTwoFactorAsync(Guid clientId, IUser user, string loginProvider = null, bool bypassTwoFactor = false)
        {
            if (!bypassTwoFactor && await IsTfaEnabled(user))
            {
                if (!await IsTwoFactorClientRememberedAsync(user))
                {
                    // Store the userId for use after two factor check
                    string userId = await UserManager.GetUserIdAsync(user);
                    await Context.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, StoreTwoFactorInfo(clientId.ToString(), userId, loginProvider));

                    return SignInResult.TwoFactorRequired;
                }
            }

            // Cleanup external cookie
            if (loginProvider != null)
            {
                await Context.SignOutAsync(IdentityConstants.ExternalScheme);
            }

            return SignInResult.Success;
        }

        public async Task<TwoFactorAuthenticationInfo> GetTwoFactorInfoAsync()
        {
            AuthenticateResult result = await Context.AuthenticateAsync(IdentityConstants.TwoFactorUserIdScheme);
            if (result?.Principal != null)
            {
                return new TwoFactorAuthenticationInfo
                {
                    UserId = result.Principal.FindFirstValue(ClaimTypes.Name),
                    LoginProvider = result.Principal.FindFirstValue(ClaimTypes.AuthenticationMethod),
                    ClientId = result.Principal.FindFirstValue(ApplicationClaimTypes.ClientIdentifier)
                };
            }

            return null;
        }

        /// <summary>
        /// Used to determine if a user is considered locked out.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Whether a user is considered locked out.</returns>
        protected virtual async Task<bool> IsLockedOut(IUser user)
        {
            return UserManager.SupportsUserLockout && await UserManager.IsLockedOutAsync(user);
        }

        /// <summary>
        /// Returns a locked out SignInResult.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>A locked out SignInResult</returns>
        protected virtual async Task<SignInResult> LockedOut(IUser user)
        {
            Logger.LogWarning(3, "User {userId} is currently locked out.", await UserManager.GetUserIdAsync(user));

            return SignInResult.LockedOut;
        }

        /// <summary>
        /// Used to ensure that a user is allowed to sign in.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="user">The user</param>
        /// <returns>Null if the user should be allowed to sign in, otherwise the SignInResult why they should be denied.</returns>
        protected virtual async Task<SignInResult> PreSignInCheck(Guid clientId, IUser user)
        {
            if (!await CanSignInAsync(user))
            {
                string userId = await UserManager.GetUserIdAsync(user);
                await Context.SignInAsync(ApplicationIdentityConstants.AccountVerificationScheme, StoreAccountVerificationInfo(clientId.ToString(), userId));

                return SignInResult.NotAllowed;
            }

            if (await IsLockedOut(user))
            {
                return await LockedOut(user);
            }

            return null;
        }

        /// <summary>
        /// Used to reset a user's lockout count.
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the operation.</returns>
        protected virtual Task ResetLockout(IUser user)
        {
            if (UserManager.SupportsUserLockout)
            {
                return UserManager.ResetAccessFailedCountAsync(user);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="user">The user</param>
        public async Task<SignInResult> RegisterAsync(Guid clientId, IUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                string userId = await UserManager.GetUserIdAsync(user);
                await Context.SignInAsync(ApplicationIdentityConstants.AccountVerificationScheme, StoreAccountVerificationInfo(clientId.ToString(), userId));

                return SignInResult.Success;
            }
            catch
            {
                return SignInResult.Failed;
            }
        }

        public async Task<SignInResult> AccountVerificationSignInAsync(Guid clientId)
        {
            IUser user = await GetAccountVerificationUserAsync();
            if (user == null)
            {
                return SignInResult.Failed;
            }

            if (!await CanSignInAsync(user))
            {
                return SignInResult.NotAllowed;
            }

            if (await IsLockedOut(user))
            {
                return await LockedOut(user);
            }

            // Cleanup the account verification user id cookie
            await Context.SignOutAsync(ApplicationIdentityConstants.AccountVerificationScheme);

            return await SignInOrTwoFactorAsync(clientId, user);
        }

        public async Task<IUser> GetAccountVerificationUserAsync()
        {
            AccountVerificationInfo verificationInfo = await GetAccountVerificationInfoAsync();

            if (verificationInfo?.UserId != null)
            {
                return await UserManager.FindByIdAsync(verificationInfo.UserId);
            }

            return default;
        }

        public async Task<AccountVerificationInfo> GetAccountVerificationInfoAsync()
        {
            AuthenticateResult result = await Context.AuthenticateAsync(ApplicationIdentityConstants.AccountVerificationScheme);

            if (result?.Principal != null)
            {
                return new AccountVerificationInfo
                {
                    UserId = result.Principal.FindFirstValue(ClaimTypes.Name),
                    ClientId = result.Principal.FindFirstValue(ApplicationClaimTypes.ClientIdentifier)
                };
            }

            return null;
        }

        private static ClaimsPrincipal StoreAccountVerificationInfo(string clientId, string userId)
        {
            ClaimsIdentity identity = new ClaimsIdentity(ApplicationIdentityConstants.AccountVerificationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, userId));
            identity.AddClaim(new Claim(ApplicationClaimTypes.ClientIdentifier, clientId));

            return new ClaimsPrincipal(identity);
        }
    }

    public class TwoFactorAuthenticationInfo
    {
        public string UserId { get; set; }

        public string LoginProvider { get; set; }

        public string ClientId { get; set; }
    }

    public class AccountVerificationInfo
    {
        public string UserId { get; set; }

        public string ClientId { get; set; }
    }
}