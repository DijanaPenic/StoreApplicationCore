using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using NETCore.Encrypt;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

using Store.Common.Helpers;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Service.Common.Services.Identity;

namespace Store.WebAPI.Identity
{
    public sealed class ApplicationUserManager : UserManager<IUser>
    {
        private readonly IApplicationUserStore<IUser> _userStore;
        private readonly IApplicationLoginUserStore<IUser> _loginStore;
        private readonly IConfiguration _configuration;
        private readonly bool _isEncryptionEnabled;
        private readonly string _encryptionKey;

        public ApplicationUserManager(
            IUserStore<IUser> userStore, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<IUser> passwordHasher, 
            IEnumerable<IUserValidator<IUser>> userValidators, 
            IEnumerable<IPasswordValidator<IUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<IUser>> logger,
            IConfiguration configuration) 
            : base(userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userStore = (IApplicationUserStore<IUser>)userStore;
            _loginStore = (IApplicationLoginUserStore<IUser>)userStore;
            _configuration = configuration;

            // Encryption configuration
            IConfigurationSection twoFactorAuthConfig = _configuration.GetSection("TwoFactorAuthentication");

            _isEncryptionEnabled = twoFactorAuthConfig.GetValue<bool>("EncryptionEnabled");
            _encryptionKey = twoFactorAuthConfig.GetValue<string>("EncryptionKey");
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

        public Task<IUser> FindByLoginAsync(UserLoginInfo login, bool loginConfirmed)
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

        public Task<IPagedEnumerable<IUser>> FindUsersAsync(string searchString, bool showInactive, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, params string[] includeProperties)
        {
            if (sortOrderProperty == null)
            {
                throw new ArgumentNullException(nameof(sortOrderProperty)); 
            }

            return _userStore.FindUsersAsync(searchString, showInactive, sortOrderProperty, isDescendingSortOrder, pageNumber, pageSize, includeProperties);
        }

        public Task<IUser> FindUserByIdAsync(Guid id, params string[] includeProperties)
        {
            if (GuidHelper.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _userStore.FindUserByIdAsync(id, includeProperties);
        }

        public override string GenerateNewAuthenticatorKey()
        {
            string originalAuthenticatorKey = base.GenerateNewAuthenticatorKey();

            string encryptedKey = _isEncryptionEnabled ? EncryptProvider.AESEncrypt(originalAuthenticatorKey, _encryptionKey) : originalAuthenticatorKey;

            return encryptedKey;
        }

        public override async Task<string> GetAuthenticatorKeyAsync(IUser user)
        {
            string databaseKey = await base.GetAuthenticatorKeyAsync(user);

            if (databaseKey == null)
            {
                return null;
            }

            string originalAuthenticatorKey = _isEncryptionEnabled ? EncryptProvider.AESDecrypt(databaseKey, _encryptionKey) : databaseKey;

            return originalAuthenticatorKey;
        }

        protected override string CreateTwoFactorRecoveryCode()
        {
            string originalRecoveryCode = base.CreateTwoFactorRecoveryCode();

            string encryptedRecoveryCode = _isEncryptionEnabled ? EncryptProvider.AESEncrypt(originalRecoveryCode, _encryptionKey) : originalRecoveryCode;

            return encryptedRecoveryCode;
        }

        public override async Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(IUser user, int number)
        {
            IEnumerable<string> recoveryCodes = await base.GenerateNewTwoFactorRecoveryCodesAsync(user, number);

            if (!recoveryCodes.Any())
            {
                return recoveryCodes;
            }

            recoveryCodes = _isEncryptionEnabled ? recoveryCodes.Select(token => EncryptProvider.AESDecrypt(token, _encryptionKey)) : recoveryCodes;

            return recoveryCodes;
        }

        public override Task<IdentityResult> RedeemTwoFactorRecoveryCodeAsync(IUser user, string code)
        {
            if (_isEncryptionEnabled && !string.IsNullOrEmpty(code))
            {
                code = EncryptProvider.AESEncrypt(code, _encryptionKey);
            }

            return base.RedeemTwoFactorRecoveryCodeAsync(user, code);
        }
    }
}