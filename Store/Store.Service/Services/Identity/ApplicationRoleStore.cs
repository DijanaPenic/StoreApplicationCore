using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using Store.Model.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Services.Identity
{
    public class ApplicationRoleStore :
            IRoleStore<IRole>,
            IRoleClaimStore<IRole>
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public ApplicationRoleStore(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region IRoleStore<IRole> Members

        public Task<IdentityResult> CreateAsync(IRole role, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                if (role == null)
                    throw new ArgumentNullException(nameof(role));

                _unitOfWork.RoleRepository.Add(role);
                _unitOfWork.Commit();

                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message }));
            }
        }

        public Task<IdentityResult> DeleteAsync(IRole role, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                if (role == null)
                    throw new ArgumentNullException(nameof(role));

                _unitOfWork.RoleRepository.DeleteByKey(role.Id);
                _unitOfWork.Commit();

                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message }));
            }
        }

        public Task<IRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(roleId))
                throw new ArgumentNullException(nameof(roleId));

            if (!Guid.TryParse(roleId, out Guid id))
                throw new ArgumentOutOfRangeException(nameof(roleId), $"{nameof(roleId)} is not a valid GUID");

            IRole role = _unitOfWork.RoleRepository.FindByKey(id);

            return Task.FromResult(role);
        }

        public Task<IRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            IRole role = _unitOfWork.RoleRepository.FindByName(normalizedRoleName);

            return Task.FromResult(role);
        }

        public Task<string> GetNormalizedRoleNameAsync(IRole role, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(IRole role, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(IRole role, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(IRole role, string normalizedName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(IRole role, string roleName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.Name = roleName;

            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IRole role, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                if (role == null)
                    throw new ArgumentNullException(nameof(role));

                _unitOfWork.RoleRepository.Update(role);
                _unitOfWork.Commit();

                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message }));
            }
        }

        public void Dispose()
        {
            // Lifetimes of dependencies are managed by the IoC container, so disposal here is unnecessary.
        }

        #endregion

        #region IRoleClaimStore<IRole> Members

        public Task<IList<Claim>> GetClaimsAsync(IRole role, CancellationToken cancellationToken = default)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            IList<Claim> result = _unitOfWork.RoleClaimRepository.FindByRoleId(role.Id).Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();

            return Task.FromResult(result);
        }

        public Task AddClaimAsync(IRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            if (cancellationToken != null)
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

            _unitOfWork.RoleClaimRepository.Add(roleClaim);
            _unitOfWork.Commit();

            return Task.CompletedTask;
        }

        public Task RemoveClaimAsync(IRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            IRoleClaim roleClaim = _unitOfWork.RoleClaimRepository.FindByRoleId(role.Id).SingleOrDefault(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);

            if (roleClaim != null)
            {
                _unitOfWork.RoleClaimRepository.DeleteByKey(roleClaim.Id);
                _unitOfWork.Commit();
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}