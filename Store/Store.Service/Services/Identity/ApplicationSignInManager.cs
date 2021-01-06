using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using Store.Model.Common.Models.Identity;

namespace Store.Services.Identity
{
    // SignInManager<IUser> Notes
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
        public override async Task<SignInResult> PasswordSignInAsync(IUser user, string password,bool isPersistent, bool lockoutOnFailure)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            SignInResult attempt = await CheckPasswordSignInAsync(user, password, lockoutOnFailure);

            return attempt.Succeeded
                ? await SignInOrTwoFactorAsync(user, isPersistent)
                : attempt;
        }

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

            // SignInAsync - creates cookie -> not needed as application is using JWT authentication
            //await SignInAsync(user, isPersistent, loginProvider);

            return SignInResult.Success;
        }

        public override async Task<bool> IsTwoFactorClientRememberedAsync(IUser user)
        {
            string userId = await UserManager.GetUserIdAsync(user);
            AuthenticateResult result = await Context.AuthenticateAsync(IdentityConstants.TwoFactorRememberMeScheme);

            return (result?.Principal != null && result.Principal.FindFirstValue(ClaimTypes.Name) == userId);
        }

        public override async Task SignInAsync(IUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            ClaimsPrincipal userPrincipal = await CreateUserPrincipalAsync(user);

            // Review: should we guard against CreateUserPrincipal returning null?
            if (authenticationMethod != null)
            {
                userPrincipal.Identities.First().AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            }

            await Context.SignInAsync(IdentityConstants.ApplicationScheme, userPrincipal, authenticationProperties ?? new AuthenticationProperties());
        }

        private async Task<bool> IsTfaEnabled(IUser user) => UserManager.SupportsUserTwoFactor && await UserManager.GetTwoFactorEnabledAsync(user) && (await UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;

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
    }
}