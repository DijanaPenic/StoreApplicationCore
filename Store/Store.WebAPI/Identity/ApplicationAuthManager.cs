using System;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

using Store.Common.Enums;
using Store.Common.Helpers;
using Store.WebAPI.Models;
using Store.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Service.Common.Services.Identity;

namespace Store.WebAPI.Identity
{
    public class ApplicationAuthManager
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IApplicationAuthStore _authStore;
        private readonly JwtTokenConfig _jwtTokenConfig; // TODO - need to check issuer and audience
        private readonly byte[] _secret;

        public ApplicationAuthManager(ApplicationUserManager userManager, IApplicationAuthStore authStore, JwtTokenConfig jwtTokenConfig)
        {
            _userManager = userManager;
            _authStore = authStore;
            _jwtTokenConfig = jwtTokenConfig;
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }

        public async Task<JwtAuthResult> GenerateTokensAsync(string userName, Guid clientId)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName));

            if (GuidHelper.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            // Find user by username
            IUser user = await _userManager.FindByNameAsync(userName);

            // Get user roles 
            IList<string> roles = await _userManager.GetRolesAsync(user);

            // Set user claims
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, string.Join(',', roles))
            };

            // Find client by name
            IClient client = await _authStore.FindClientByIdAsync(clientId);

            // Set jwt security token
            JwtSecurityToken jwtToken = new JwtSecurityToken
            (
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(client.AccessTokenLifeTime),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature)
            );

            // Set access token
            string accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            // Set refresh token
            IUserRefreshToken refreshToken = new UserRefreshToken
            {
                UserId = user.Id,
                ClientId = client.Id,
                Value = GenerateRefreshTokenValue(),
                DateExpiresUtc = DateTime.UtcNow.AddMinutes(client.RefreshTokenLifeTime)
            };

            // Delete the existing refresh token from the database (if found)
            await _authStore.RemoveRefreshTokenAsync(user.Id, client.Id);

            // Save refresh token in the database
            await _authStore.AddRefreshTokenAsync(refreshToken);

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Roles = roles
            };
        }

        public Task RemoveRefreshTokenAsync(Guid userId, Guid clientId)
        {
            if (GuidHelper.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (GuidHelper.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            return _authStore.RemoveRefreshTokenAsync(userId, clientId);
        }

        public Task RemoveExpiredRefreshTokensAsync()
        {
            return _authStore.RemoveExpiredRefreshTokensAsync();
        }

        public async Task<ClientAuthResult> ValidateClientAuthenticationAsync(Guid clientId, string clientSecret)
        {
            if (GuidHelper.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            IClient client = await _authStore.FindClientByIdAsync(clientId);

            if(client == null)
            {
                return new ClientAuthResult($"Client '{clientId}' is not registered in the system.");
            }
            if(!client.Active)
            {
                return new ClientAuthResult($"Client '{clientId}' is not active.");
            }
            if (client.ApplicationType == ApplicationType.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    return new ClientAuthResult($"Client secret should be provided for '{clientId}'");
                }
                else if (client.Secret != HashHelper.GetSHA512Hash(clientSecret))
                {
                    return new ClientAuthResult($"Client secret is not valid for '{clientId}'");
                }
            }

            return new ClientAuthResult();
        }

        private string GenerateRefreshTokenValue()
        {
            byte[] randomNumber = new byte[32];
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}