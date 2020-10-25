﻿using System;
using System.Threading.Tasks;

using Store.Common.Helpers;
using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;
using Store.Service.Common.Services.Identity;

namespace Store.Services.Identity
{
    public class ApplicationAuthStore : IApplicationAuthStore
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public ApplicationAuthStore(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region IUserRefreshToken Members

        public async Task AddRefreshTokenAsync(IUserRefreshToken refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));

            await _unitOfWork.UserRefreshTokenRepository.AddAsync(refreshToken);

            _unitOfWork.Commit();
        }

        public async Task RemoveRefreshTokenAsync(Guid refreshTokenId)
        {
            if (GuidHelper.IsNullOrEmpty(refreshTokenId))
                throw new ArgumentNullException(nameof(refreshTokenId));

            await _unitOfWork.UserRefreshTokenRepository.DeleteByKeyAsync(refreshTokenId);

            _unitOfWork.Commit();
        }

        public async Task RemoveRefreshTokenAsync(Guid userId, Guid clientId)
        {
            if (GuidHelper.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (GuidHelper.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            await _unitOfWork.UserRefreshTokenRepository.DeleteAsync(userId, clientId);

            _unitOfWork.Commit();
        }

        public Task RemoveExpiredRefreshTokensAsync()
        {
            return _unitOfWork.UserRefreshTokenRepository.DeleteExpiredAsync();
        }

        public Task<IUserRefreshToken> FindRefreshTokenByIdAsync(Guid refreshTokenId)
        {
            if (GuidHelper.IsNullOrEmpty(refreshTokenId))
                throw new ArgumentNullException(nameof(refreshTokenId));

            return _unitOfWork.UserRefreshTokenRepository.FindByKeyAsync(refreshTokenId);
        }

        public Task<IUserRefreshToken> FindRefreshTokenByValueAsync(string refreshTokenValue)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
                throw new ArgumentNullException(nameof(refreshTokenValue));

            return _unitOfWork.UserRefreshTokenRepository.FindByValueAsync(refreshTokenValue);
        }

        #endregion

        #region IClientStore Members

        public Task<IClient> FindClientByIdAsync(Guid clientId)
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
            // Dispose does nothing since we want Autofac to manage the lifecycle of our Unit of Work
        }

        #endregion
    }
}