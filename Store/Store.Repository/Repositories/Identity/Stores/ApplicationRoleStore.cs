﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using X.PagedList;
using Microsoft.AspNetCore.Identity;

using Store.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core;
using Store.Repository.Common.Repositories.Identity.Stores;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;

namespace Store.Repositories.Identity.Stores
{
    public sealed class ApplicationRoleStore :
            IApplicationRoleClaimStore,
            IApplicationRoleStore
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationRoleStore(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region IRoleStore<IRole> Members

        public async Task<IdentityResult> CreateAsync(IRole role, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (role == null)
                    throw new ArgumentNullException(nameof(role));

                await _unitOfWork.RoleRepository.AddAsync(role);
                await _unitOfWork.CommitAsync();

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message });
            }
        }

        public async Task<IdentityResult> DeleteAsync(IRole role, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (role == null)
                    throw new ArgumentNullException(nameof(role));

                await _unitOfWork.RoleRepository.DeleteByKeyAsync(role.Id);
                await _unitOfWork.CommitAsync();

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message });
            }
        }

        public async Task<IRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(roleId))
                throw new ArgumentNullException(nameof(roleId));

            if (!Guid.TryParse(roleId, out Guid id))
                throw new ArgumentOutOfRangeException(nameof(roleId), $"{nameof(roleId)} is not a valid GUID");

            IRole role = await _unitOfWork.RoleRepository.FindByKeyAsync(id);

            return role;
        }

        public async Task<IRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            IRole role = await _unitOfWork.RoleRepository.FindByNameAsync(normalizedRoleName);

            return role;
        }

        public Task<string> GetNormalizedRoleNameAsync(IRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(IRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(IRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(IRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(IRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.Name = roleName;

            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(IRole role, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (role == null)
                    throw new ArgumentNullException(nameof(role));

                await _unitOfWork.RoleRepository.UpdateAsync(role);
                await _unitOfWork.CommitAsync();

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message });
            }
        }

        #endregion

        #region IRoleClaimStore<IRole> Members

        public async Task<IList<Claim>> GetClaimsAsync(IRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            IList<Claim> result = (await _unitOfWork.RoleClaimRepository.FindByRoleIdAsync(role.Id)).Select(rc => new Claim(rc.ClaimType, rc.ClaimValue)).ToList();

            return result;
        }

        public async Task AddClaimAsync(IRole role, Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            IRoleClaim roleClaim = new RoleClaim
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                RoleId = role.Id
            };

            await _unitOfWork.RoleClaimRepository.AddAsync(roleClaim);
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoveClaimAsync(IRole role, Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            IRoleClaim roleClaim = (await _unitOfWork.RoleClaimRepository.FindByRoleIdAsync(role.Id)).SingleOrDefault(rc => rc.ClaimType == claim.Type && rc.ClaimValue == claim.Value);

            if (roleClaim != null)
            {
                await _unitOfWork.RoleClaimRepository.DeleteByKeyAsync(roleClaim.Id);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task RemoveClaimsAsync(IRoleClaimFilteringParameters filter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await _unitOfWork.RoleClaimRepository.DeleteAsync(filter);
            await _unitOfWork.CommitAsync();
        }

        public Task<IPagedList<IRoleClaim>> FindClaimsAsync(IRoleClaimFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _unitOfWork.RoleClaimRepository.FindAsync(filter, paging, sorting);
        }

        #endregion

        #region IApplicationRoleStore<IRole> Members

        public Task<IRole> FindRoleByKeyAsync(Guid key, IOptionsParameters options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _unitOfWork.RoleRepository.FindByKeyAsync(key, options);
        }

        public async Task<IEnumerable<IRole>> FindByNameAsync(string[] normalizedRoleNames, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (normalizedRoleNames?.Length == 0)
                throw new ArgumentNullException(nameof(normalizedRoleNames));

            IEnumerable<IRole> roles = await _unitOfWork.RoleRepository.FindByNameAsync(normalizedRoleNames);

            return roles;
        }

        public async Task<int> GetCountByRoleNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            return await _unitOfWork.UserRoleRepository.GetCountByRoleNameAsync(normalizedRoleName);
        }

        public Task<IPagedList<IRole>> FindRolesAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _unitOfWork.RoleRepository.FindAsync(filter, paging, sorting, options);
        }

        public Task<IPagedList<IRole>> FindRolesBySectionAsync(IPermissionFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _unitOfWork.RoleRepository.FindBySectionAsync(filter, paging, sorting);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // Lifetimes of dependencies are managed by the IoC container, so disposal here is unnecessary.
        }

        #endregion
    }
}