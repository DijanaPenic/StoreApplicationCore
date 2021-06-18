using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;
using NETCore.Encrypt;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

using Store.Common.Helpers;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Filtering;
using Store.Service.Options;
using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Repositories.Identity.Stores;

namespace Store.Services.Identity
{
    public sealed class ApplicationUserManager : UserManager<IUser>
    {
        private readonly IApplicationUserStore _userStore;
        private readonly IApplicationLoginUserStore _loginStore;
        private readonly IUserPasswordStore<IUser> _passwordStore;
        private readonly IUserRoleStore<IUser> _userRoleStore;
        private readonly TwoFactorAuthOptions _twoFactorAuthConfig;

        public ApplicationUserManager(
            IUserStore<IUser> userStore, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<IUser> passwordHasher, 
            IEnumerable<IUserValidator<IUser>> userValidators, 
            IEnumerable<IPasswordValidator<IUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<ApplicationUserManager> logger,
            IOptions<TwoFactorAuthOptions> twoFactorAuthOptions) 
            : base(userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userStore = (IApplicationUserStore)userStore;
            _loginStore = (IApplicationLoginUserStore)userStore;
            _passwordStore = (IUserPasswordStore<IUser>)userStore;
            _userRoleStore = (IUserRoleStore<IUser>)userStore;
            _twoFactorAuthConfig = twoFactorAuthOptions.Value;
        }

        public async Task<IdentityResult> AddToRoleAsync(string normalizedUserName, string role)
        {
            IUser user = await _userStore.FindByNameAsync(normalizedUserName, CancellationToken);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "User Update", Description = "User not found." });
            }

            string normalizedRole = NormalizeName(role);
            if (await _userRoleStore.IsInRoleAsync(user, normalizedRole, CancellationToken))
            {
                Logger.LogWarning(5, "User {userId} is already in role {role}.", await GetUserIdAsync(user), role);

                return IdentityResult.Failed(ErrorDescriber.UserAlreadyInRole(role));
            }

            await _userRoleStore.AddToRoleAsync(user, normalizedRole, CancellationToken);

            return await UpdateUserAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(IUser user, string newPassword)
        {
            foreach (IPasswordValidator<IUser> passwordValidator in PasswordValidators)
            {
                IdentityResult result = await passwordValidator.ValidateAsync(this, user, newPassword);
                if (!result.Succeeded)
                {
                    return result;
                }
            }

            string newPasswordHash = PasswordHasher.HashPassword(user, newPassword);

            await _passwordStore.SetPasswordHashAsync(user, newPasswordHash, CancellationToken);

            return await _userStore.UpdateAsync(user, CancellationToken);
        }

        // User can initiate external login request multiple times, so need to support update of the existing login record
        public async Task<IdentityResult> AddOrUpdateLoginAsync(IUser user, UserLoginInfo login, string token)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            try
            {
                IUserLogin loginEntity = await _loginStore.FindLoginAsync(login, CancellationToken);

                // Insert new login
                if (loginEntity == null)
                {
                    await _loginStore.AddLoginAsync(user, login, token, CancellationToken);

                    return IdentityResult.Success;
                }

                // Update the existing login
                loginEntity.Token = token;

                await _loginStore.UpdateLoginAsync(loginEntity, CancellationToken);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message });
            }
        }

        public Task<IUser> FindUserByLoginAsync(UserLoginInfo login, bool loginConfirmed)
        {
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            return _loginStore.FindByLoginAsync(login, loginConfirmed, CancellationToken);
        }

        public async Task<IdentityResult> ConfirmLoginAsync(IUser user, string token)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            try
            {
                IUserLogin login = await _loginStore.FindLoginAsync(user, token, CancellationToken);
                if (login == null)
                {
                    return IdentityResult.Failed(new IdentityError { Code = "External Login", Description = "External Login not found." });
                }
                else if (login.IsConfirmed)
                {
                    return IdentityResult.Failed(new IdentityError { Code = "External Login", Description = "External Login is already confirmed." });
                }

                await _loginStore.ConfirmLoginAsync(login, CancellationToken);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message });
            }
        }

        public async Task<IList<UserLoginInfo>> FindLoginsAsync(IUser user, bool loginConfirmed)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            IList<UserLoginInfo> result = await _loginStore.FindLoginsAsync(user, loginConfirmed, CancellationToken);

            return result;
        }

        public Task<IPagedList<IUser>> FindUsersAsync(IUserFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            return _userStore.FindUsersAsync(filter, paging, sorting, options, CancellationToken);
        }

        public Task<IUser> FindUserByKeyAsync(Guid key, IOptionsParameters options = null)
        {
            if (GuidHelper.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _userStore.FindUserByKeyAsync(key, options, CancellationToken);
        }

        public async Task<IdentityResult> ApproveUserAsync(IUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _userStore.ApproveUserAsync(user, CancellationToken);

            return await _userStore.UpdateAsync(user, CancellationToken);
        }

        public async Task<IdentityResult> DisapproveUserAsync(IUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _userStore.DisapproveUserAsync(user, CancellationToken);

            return await _userStore.UpdateAsync(user, CancellationToken);
        }

        public override string GenerateNewAuthenticatorKey()
        {
            string originalAuthenticatorKey = base.GenerateNewAuthenticatorKey();

            string encryptedKey = _twoFactorAuthConfig.EncryptionEnabled ? EncryptProvider.AESEncrypt(originalAuthenticatorKey, _twoFactorAuthConfig.EncryptionKey) : originalAuthenticatorKey;

            return encryptedKey;
        }

        public override async Task<string> GetAuthenticatorKeyAsync(IUser user)
        {
            string databaseKey = await base.GetAuthenticatorKeyAsync(user);

            if (databaseKey == null)
            {
                return null;
            }

            string originalAuthenticatorKey = _twoFactorAuthConfig.EncryptionEnabled ? EncryptProvider.AESDecrypt(databaseKey, _twoFactorAuthConfig.EncryptionKey) : databaseKey;

            return originalAuthenticatorKey;
        }

        protected override string CreateTwoFactorRecoveryCode()
        {
            string originalRecoveryCode = base.CreateTwoFactorRecoveryCode();

            string encryptedRecoveryCode = _twoFactorAuthConfig.EncryptionEnabled ? EncryptProvider.AESEncrypt(originalRecoveryCode, _twoFactorAuthConfig.EncryptionKey) : originalRecoveryCode;

            return encryptedRecoveryCode;
        }

        public override async Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(IUser user, int number)
        {
            IEnumerable<string> recoveryCodes = await base.GenerateNewTwoFactorRecoveryCodesAsync(user, number);

            if (!recoveryCodes.Any())
            {
                return recoveryCodes;
            }

            recoveryCodes = _twoFactorAuthConfig.EncryptionEnabled ? recoveryCodes.Select(token => EncryptProvider.AESDecrypt(token, _twoFactorAuthConfig.EncryptionKey)) : recoveryCodes;

            return recoveryCodes;
        }

        public override Task<IdentityResult> RedeemTwoFactorRecoveryCodeAsync(IUser user, string code)
        {
            if (_twoFactorAuthConfig.EncryptionEnabled && !string.IsNullOrEmpty(code))
            {
                code = EncryptProvider.AESEncrypt(code, _twoFactorAuthConfig.EncryptionKey);
            }

            return base.RedeemTwoFactorRecoveryCodeAsync(user, code);
        }
    }
}