using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using Store.Models.Identity;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Repository.Common.Core.Dapper;
using Store.Repository.Common.Repositories.Identity.Stores;

namespace Store.Repositories.Identity.Stores
{
    public class ApplicationUserStore :
            IUserPasswordStore<IUser>,
            IUserEmailStore<IUser>,
            IUserRoleStore<IUser>,
            IUserSecurityStampStore<IUser>,
            IUserClaimStore<IUser>,
            IUserTwoFactorStore<IUser>,
            IUserPhoneNumberStore<IUser>,
            IUserLockoutStore<IUser>,
            IUserAuthenticationTokenStore<IUser>,
            IUserAuthenticatorKeyStore<IUser>,
            IUserTwoFactorRecoveryCodeStore<IUser>,

            // Custom implementation
            IApplicationUserStore,
            IApplicationLoginUserStore
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";

        public ApplicationUserStore(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region IUserStore<IUser> Members

        public async Task<IdentityResult> CreateAsync(IUser user, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                await _unitOfWork.UserRepository.AddAsync(user);
                _unitOfWork.Commit();

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message });
            }
        }

        public async Task<IdentityResult> DeleteAsync(IUser user, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                await _unitOfWork.UserRepository.DeleteByKeyAsync(user.Id);
                _unitOfWork.Commit();

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message });
            }
        }

        public async Task<IUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            if (!Guid.TryParse(userId, out Guid id))
                throw new ArgumentOutOfRangeException(nameof(userId), $"{nameof(userId)} is not a valid GUID");

            IUser user = await _unitOfWork.UserRepository.FindByKeyAsync(id);

            return user;
        }

        public async Task<IUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IUser user = await _unitOfWork.UserRepository.FindByNormalizedUserNameAsync(normalizedUserName);

            return user;
        }

        public Task<string> GetNormalizedUserNameAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(IUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(IUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.UserName = userName;

            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(IUser user, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                await _unitOfWork.UserRepository.UpdateAsync(user);
                _unitOfWork.Commit();

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message });
            }
        }

        #endregion

        #region IUserPasswordStore<IUser> Members

        public Task SetPasswordHashAsync(IUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        #endregion

        #region IUserEmailStore<IUser> Members

        public Task SetEmailAsync(IUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.Email = email;

            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(IUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.EmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public async Task<IUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(normalizedEmail))
                throw new ArgumentNullException(nameof(normalizedEmail));

            IUser user = await _unitOfWork.UserRepository.FindByNormalizedEmailAsync(normalizedEmail);

            return user;
        }

        public Task<string> GetNormalizedEmailAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(IUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }

        #endregion

        #region IUserLoginStore<IUser> Members

        public async Task AddLoginAsync(IUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (login == null)
                throw new ArgumentNullException(nameof(login));

            IUserLogin loginEntity = new UserLogin
            {
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
                ProviderKey = login.ProviderKey,
                UserId = user.Id,
                IsConfirmed = true      // Token is not issued, so confirmation is not required
            };

            await _unitOfWork.UserLoginRepository.AddAsync(loginEntity);
            _unitOfWork.Commit();
        }

        public async Task RemoveLoginAsync(IUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ArgumentNullException(nameof(providerKey));

            await _unitOfWork.UserLoginRepository.DeleteByKeyAsync(new UserLoginKey { LoginProvider = loginProvider, ProviderKey = providerKey });
            _unitOfWork.Commit();
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            IList<UserLoginInfo> result = (await _unitOfWork.UserLoginRepository.FindByUserIdAsync(user.Id))
                .Select(ul => new UserLoginInfo(ul.LoginProvider, ul.ProviderKey, ul.ProviderDisplayName))
                .ToList();

            return result;
        }

        public async Task<IUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ArgumentNullException(nameof(providerKey));

            IUserLogin login = await _unitOfWork.UserLoginRepository.FindByKeyAsync(new UserLoginKey { LoginProvider = loginProvider, ProviderKey = providerKey });
            if (login == null)
                return default;

            IUser user = await _unitOfWork.UserRepository.FindByKeyAsync(login.UserId);

            return user;
        }

        #endregion

        #region IApplicationUserLoginStore<IUser> Members

        public async Task AddLoginAsync(IUser user, UserLoginInfo login, string token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (login == null)
                throw new ArgumentNullException(nameof(login));

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            IUserLogin loginEntity = new UserLogin
            {
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
                ProviderKey = login.ProviderKey,
                UserId = user.Id,
                Token = token,
                IsConfirmed = false
            };

            await _unitOfWork.UserLoginRepository.AddAsync(loginEntity);
            _unitOfWork.Commit();
        }

        public async Task UpdateLoginAsync(IUserLogin login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (login == null)
                throw new ArgumentNullException(nameof(login));

            await _unitOfWork.UserLoginRepository.UpdateAsync(login);
            _unitOfWork.Commit();
        }

        public async Task<IUserLogin> FindLoginAsync(UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (login == null)
                throw new ArgumentNullException(nameof(login));

            IUserLogin result = await _unitOfWork.UserLoginRepository.FindByKeyAsync(new UserLoginKey { LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey });

            return result;
        }

        public async Task<IUserLogin> FindLoginAsync(IUser user, string token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            IUserLogin result = await _unitOfWork.UserLoginRepository.FindAsync(user.Id, token);

            return result;
        }

        public async Task<IUser> FindByLoginAsync(UserLoginInfo login, bool loginConfirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (login == null)
                throw new ArgumentNullException(nameof(login));

            IUserLogin loginEntity = await _unitOfWork.UserLoginRepository.FindAsync(new UserLoginKey { LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey }, loginConfirmed);
            if (loginEntity == null)
                return default;

            IUser result = await _unitOfWork.UserRepository.FindByKeyAsync(loginEntity.UserId);

            return result;
        }

        public async Task ConfirmLoginAsync(IUserLogin login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (login == null)
                throw new ArgumentNullException(nameof(login));

            login.IsConfirmed = true;

            await _unitOfWork.UserLoginRepository.UpdateAsync(login);
            _unitOfWork.Commit();
        }

        public async Task<IList<UserLoginInfo>> FindLoginsAsync(IUser user, bool loginConfirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            IList<UserLoginInfo> result = (await _unitOfWork.UserLoginRepository.FindByUserIdAsync(user.Id, loginConfirmed))
                .Select(ul => new UserLoginInfo(ul.LoginProvider, ul.ProviderKey, ul.ProviderDisplayName))
                .ToList();

            return result;
        }

        #endregion

        #region IUserRoleStore<IUser> Members

        public async Task AddToRoleAsync(IUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            await _unitOfWork.UserRoleRepository.AddAsync(user.Id, roleName);
            _unitOfWork.Commit();
        }

        public async Task RemoveFromRoleAsync(IUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            await _unitOfWork.UserRoleRepository.DeleteAsync(user.Id, roleName);
            _unitOfWork.Commit();
        }

        public async Task<IList<string>> GetRolesAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            IList<string> result = (await _unitOfWork.UserRoleRepository.GetRoleNamesByUserIdAsync(user.Id)).ToList();

            return result;
        }

        public async Task<bool> IsInRoleAsync(IUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            bool result = (await _unitOfWork.UserRoleRepository.GetRoleNamesByUserIdAsync(user.Id)).Any(r => r == roleName);

            return result;
        }

        public async Task<IList<IUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            IList<IUser> result = (await _unitOfWork.UserRoleRepository.GetUsersByRoleNameAsync(roleName)).ToList();

            return result;
        }

        #endregion

        #region IUserSecurityStampStore<IUser> Members
        public Task SetSecurityStampAsync(IUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }

        #endregion

        #region IUserClaimStore<IUser> Members

        public async Task<IList<Claim>> GetClaimsAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            IList<Claim> result = (await _unitOfWork.UserClaimRepository.GetByUserIdAsync(user.Id)).Select(uc => new Claim(uc.ClaimType, uc.ClaimValue)).ToList();

            return result;
        }

        public Task AddClaimsAsync(IUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            IEnumerable<IUserClaim> userClaims = claims.Select(c => GetUserClaimEntity(c, user.Id));

            if (userClaims.Any())
            {
                userClaims.ToList().ForEach(async userClaim =>
                {
                    await _unitOfWork.UserClaimRepository.AddAsync(userClaim);
                });

                _unitOfWork.Commit();
            }

            return Task.CompletedTask;
        }

        public async Task ReplaceClaimAsync(IUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            if (newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

            IUserClaim claimEntity = (await _unitOfWork.UserClaimRepository.GetByUserIdAsync(user.Id)).SingleOrDefault(uc => uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value);

            if (claimEntity != null)
            {
                claimEntity.ClaimType = newClaim.Type;
                claimEntity.ClaimValue = newClaim.Value;

                await _unitOfWork.UserClaimRepository.UpdateAsync(claimEntity);
                _unitOfWork.Commit();
            }
        }

        public async Task RemoveClaimsAsync(IUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            IEnumerable<IUserClaim> userClaims = await _unitOfWork.UserClaimRepository.GetByUserIdAsync(user.Id);

            if (claims.Any())
            {
                claims.ToList().ForEach(async userClaim =>
                {
                    IUserClaim userClaimEntity = userClaims.SingleOrDefault(uc => uc.ClaimType == userClaim.Type && uc.ClaimValue == userClaim.Value);
                    await _unitOfWork.UserClaimRepository.DeleteByKeyAsync(userClaimEntity.Id);
                });

                _unitOfWork.Commit();
            }
        }

        public async Task<IList<IUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            IList<IUser> result = (await _unitOfWork.UserClaimRepository.GetUsersForClaimAsync(claim.Type, claim.Value)).ToList();

            return result;
        }

        #endregion

        #region IUserAuthenticationTokenStore<IUser> Members

        public async Task SetTokenAsync(IUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            IUserToken userToken = await _unitOfWork.UserTokenRepository.FindByKeyAsync(new UserTokenKey { UserId = user.Id, LoginProvider = loginProvider, Name = name });
            if (userToken == null)
            {
                userToken = new UserToken
                {
                    LoginProvider = loginProvider,
                    Name = name,
                    Value = value,
                    UserId = user.Id
                };

                await _unitOfWork.UserTokenRepository.AddAsync(userToken);
                _unitOfWork.Commit();
            }
            else
            {
                userToken.Value = value;

                await _unitOfWork.UserTokenRepository.UpdateAsync(userToken);
                _unitOfWork.Commit();
            }
        }

        public async Task RemoveTokenAsync(IUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            IUserToken userToken = await _unitOfWork.UserTokenRepository.FindByKeyAsync(new UserTokenKey { UserId = user.Id, LoginProvider = loginProvider, Name = name });
            
            if (userToken != null)
            {
                await _unitOfWork.UserTokenRepository.DeleteByKeyAsync(userToken);
                _unitOfWork.Commit();
            }
        }

        public async Task<string> GetTokenAsync(IUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            IUserToken userToken = await _unitOfWork.UserTokenRepository.FindByKeyAsync(new UserTokenKey { UserId = user.Id, LoginProvider = loginProvider, Name = name });

            return userToken?.Value;
        }

        #endregion

        #region IUserTwoFactorStore<IUser> Members

        public Task SetTwoFactorEnabledAsync(IUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.TwoFactorEnabled = enabled;

            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.TwoFactorEnabled);
        }

        #endregion

        #region IUserPhoneNumberStore<IUser> Members

        public Task SetPhoneNumberAsync(IUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(IUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }

        #endregion

        #region IUserLockoutStore<IUser> Members

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            DateTime? lockoutEndDateUtc = user.LockoutEndDateUtc;
            DateTimeOffset? result = default;

            if (lockoutEndDateUtc.HasValue)
            {
                lockoutEndDateUtc = user.LockoutEndDateUtc;
                result = new DateTimeOffset(DateTime.SpecifyKind(lockoutEndDateUtc.Value, DateTimeKind.Utc));
            }

            return Task.FromResult(result);
        }

        public Task SetLockoutEndDateAsync(IUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LockoutEndDateUtc = lockoutEnd.HasValue ? lockoutEnd.Value.DateTime : (DateTime?)null;

            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(++user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(IUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LockoutEnabled = enabled;

            return Task.CompletedTask;
        }

        #endregion

        #region IApplicationUserStore<IUser> Members

        public Task<IPagedEnumerable<IUser>> FindUsersAsync(IUserFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _unitOfWork.UserRepository.FindAsync(filter, paging, sorting, options);
        }

        public Task<IUser> FindUserByIdAsync(Guid id, IOptionsParameters options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _unitOfWork.UserRepository.FindByKeyAsync(id, options); 
        }

        public Task ApproveUserAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.IsApproved = true;

            return Task.CompletedTask;
        }

        public Task DisapproveUserAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.IsApproved = false;

            return Task.CompletedTask;
        }

        #endregion

        #region IUserAuthenticatorKeyStore<IUser, Guid> Members

        public async Task<string> GetAuthenticatorKeyAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            string userToken = await GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);

            return userToken;
        }

        public Task SetAuthenticatorKeyAsync(IUser user, string key, CancellationToken cancellationToken)
        {
            return SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
        }

        #endregion

        #region IUserTwoFactorRecoveryCodeStore<IUser> Members

        public virtual async Task<int> CountCodesAsync(IUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            string mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? string.Empty;
            if (mergedCodes.Length > 0)
            {
                return mergedCodes.Split(';').Length;
            }

            return 0;
        }

        public virtual async Task<bool> RedeemCodeAsync(IUser user, string code, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            string mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? string.Empty;
            string[] splitCodes = mergedCodes.Split(';');

            if (splitCodes.Contains(code))
            {
                var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                
                return true;
            }

            return false;
        }

        public virtual Task ReplaceCodesAsync(IUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            string mergedCodes = string.Join(";", recoveryCodes);

            return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // Lifetimes of dependencies are managed by the IoC container, so disposal here is unnecessary.
        }

        #endregion

        #region Private Methods

        private static IUserClaim GetUserClaimEntity(Claim value, Guid userId)
        {
            return value == null
                ? default
                : new UserClaim
                {
                    ClaimType = value.Type,
                    ClaimValue = value.Value,
                    UserId = userId
                };
        }

        #endregion
    }
}