using System;
using System.Threading.Tasks;

using Store.Common.Helpers;
using Store.Common.Parameters.Options;
using Store.Repository.Common.Core;
using Store.Repository.Common.Repositories.Identity.Stores;
using Store.Model.Common.Models.Identity;

namespace Store.Repositories.Identity.Stores
{
    internal class ApplicationAuthStore : IApplicationAuthStore
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationAuthStore(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region IUserRefreshToken Members

        public async Task AddRefreshTokenAsync(IUserRefreshToken refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));

            await _unitOfWork.UserRefreshTokenRepository.AddAsync(refreshToken);

            await _unitOfWork.CommitAsync();
        }

        public async Task RemoveRefreshTokenByKeyAsync(IUserRefreshTokenKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            await _unitOfWork.UserRefreshTokenRepository.DeleteByKeyAsync(key);

            await _unitOfWork.CommitAsync();
        }

        public async Task RemoveExpiredRefreshTokensAsync()
        {
            await _unitOfWork.UserRefreshTokenRepository.DeleteExpiredAsync();

            await _unitOfWork.CommitAsync();
        }

        public Task<IUserRefreshToken> FindRefreshTokenByKeyAsync(IUserRefreshTokenKey key, IOptionsParameters options)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _unitOfWork.UserRefreshTokenRepository.FindByKeyAsync(key, options);
        }

        public Task<IUserRefreshToken> FindRefreshTokenByValueAsync(string refreshTokenValue)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
                throw new ArgumentNullException(nameof(refreshTokenValue));

            return _unitOfWork.UserRefreshTokenRepository.FindByValueAsync(refreshTokenValue);
        }

        #endregion

        #region IClientStore Members

        public Task<IClient> FindClientByKeyAsync(Guid clientId)
        {
            if (GuidHelper.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            return _unitOfWork.ClientRepository.FindByKeyAsync(clientId);
        }

        public Task<IClient> FindClientByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            return _unitOfWork.ClientRepository.FindByNameAsync(name);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // Dispose does nothing since we want IoC container to manage the lifecycle of our Unit of Work
        }

        #endregion
    }
}