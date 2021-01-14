using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using Store.Service.Constants;
using Store.Model.Common.Models.Identity;
using System.Collections.Generic;

namespace Store.Services.Identity
{
    // SignInManager<IUser> Notes:
    // CheckPasswordSignInAsync - this method doesn't perform 2fa check.
    // PasswordSignInAsync - this method performs 2fa check, but also generates the ".AspNetCore.Identity.Application" cookie. Cookie creation cannot be disabled (SignInManager is heavily dependant on cookies - by design).

    public sealed class ApplicationSignInManager : SignInManager<IUser>
    {
        public ApplicationSignInManager(
            ApplicationUserManager userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<IUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor, 
            ILogger<ApplicationSignInManager> logger, 
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<IUser> confirmation) 
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public async Task<SignInResult> AccountVerificationSignInAsync()
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

            return await SignInOrTwoFactorAsync(user, false);
        }

        public async Task<IUser> GetAccountVerificationUserAsync()
        {
            AccountVerificationInfo verificationInfo = await RetrieveAccountVerificationInfoAsync();

            if (verificationInfo?.UserId != null)
            {
                return await UserManager.FindByIdAsync(verificationInfo.UserId);
            }

            return default;
        }

        public override async Task<SignInResult> PasswordSignInAsync(IUser user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            SignInResult passwordAttempt = await CheckPasswordSignInAsync(user, password, lockoutOnFailure);

            return passwordAttempt.Succeeded ? await SignInOrTwoFactorAsync(user, isPersistent) : passwordAttempt;
        }
        
        protected override async Task<SignInResult> PreSignInCheck(IUser user)
        {
            if (!await CanSignInAsync(user))
            {
                string userId = await UserManager.GetUserIdAsync(user);
                await Context.SignInAsync(ApplicationIdentityConstants.AccountVerificationScheme, StoreAccountVerificationInfo(userId));

                return SignInResult.NotAllowed;
            }
            if (await IsLockedOut(user))
            {
                return await LockedOut(user);
            }

            return null;
        }

        // isPersistent - not needed (application is using JWT authentication)
        protected override async Task<SignInResult> SignInOrTwoFactorAsync(IUser user, bool isPersistent, string loginProvider = null, bool bypassTwoFactor = false)
        {
            if (!bypassTwoFactor && await IsTfaEnabled(user))
            {
                if (!await IsTwoFactorClientRememberedAsync(user))
                {
                    // Store the userId for use after two factor check
                    string userId = await UserManager.GetUserIdAsync(user);
                    await Context.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, StoreTwoFactorInfo(userId, loginProvider)); 

                    return SignInResult.TwoFactorRequired;
                }
            }

            // Cleanup external cookie
            if (loginProvider != null)
            {
                await Context.SignOutAsync(IdentityConstants.ExternalScheme);
            }

            // Not needed as application is using JWT authentication
            //if (loginProvider == null)
            //{
            //    await SignInWithClaimsAsync(user, isPersistent, new Claim[] { new Claim("amr", "pwd") });
            //}
            //else
            //{
            //    await SignInAsync(user, isPersistent, loginProvider);
            //}

            return SignInResult.Success;
        }

        public override Task SignInWithClaimsAsync(IUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
        {
            // Not needed as application is using JWT authentication

            //ClaimsPrincipal userPrincipal = await CreateUserPrincipalAsync(user);

            //foreach (var claim in additionalClaims)
            //{
            //    userPrincipal.Identities.First().AddClaim(claim);
            //}
            //await Context.SignInAsync(IdentityConstants.ApplicationScheme,
            //    userPrincipal,
            //    authenticationProperties ?? new AuthenticationProperties());

            return Task.CompletedTask;
        }

        public async Task<SignInResult> RegisterAsync(IUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                string userId = await UserManager.GetUserIdAsync(user);
                await Context.SignInAsync(ApplicationIdentityConstants.AccountVerificationScheme, StoreAccountVerificationInfo(userId));

                return SignInResult.Success;
            }
            catch
            {
                return SignInResult.Failed;
            }
        }

        public override async Task SignOutAsync()
        {
            //await Context.SignOutAsync(IdentityConstants.ApplicationScheme);
            await Context.SignOutAsync(IdentityConstants.ExternalScheme);
            await Context.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);
            await Context.SignOutAsync(ApplicationIdentityConstants.AccountVerificationScheme);
        }

        private async Task<bool> IsTfaEnabled(IUser user) => UserManager.SupportsUserTwoFactor && await UserManager.GetTwoFactorEnabledAsync(user) && (await UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;

        private static ClaimsPrincipal StoreAccountVerificationInfo(string userId)
        {
            ClaimsIdentity identity = new ClaimsIdentity(ApplicationIdentityConstants.AccountVerificationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, userId));

            return new ClaimsPrincipal(identity);
        }

        private async Task<AccountVerificationInfo> RetrieveAccountVerificationInfoAsync()
        {
            AuthenticateResult result = await Context.AuthenticateAsync(ApplicationIdentityConstants.AccountVerificationScheme);

            if (result?.Principal != null)
            {
                return new AccountVerificationInfo
                {
                    UserId = result.Principal.FindFirstValue(ClaimTypes.Name)
                };
            }

            return null;
        }

        private static ClaimsPrincipal StoreTwoFactorInfo(string userId, string loginProvider)
        {
            ClaimsIdentity identity = new ClaimsIdentity(IdentityConstants.TwoFactorUserIdScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, userId));

            if (loginProvider != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, loginProvider));
            }

            return new ClaimsPrincipal(identity);
        }

        internal class AccountVerificationInfo
        {
            public string UserId { get; set; }
        }
    }
}