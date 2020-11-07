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
            _configuration = configuration;

            // Encryption configuration
            IConfigurationSection twoFactorAuthenticationConfig = _configuration.GetSection("TwoFactorAuthentication");

            _isEncryptionEnabled = twoFactorAuthenticationConfig.GetValue<bool>("EncryptionEnabled");
            _encryptionKey = twoFactorAuthenticationConfig.GetValue<string>("EncryptionKey");
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