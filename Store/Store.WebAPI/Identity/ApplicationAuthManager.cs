﻿using System;
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
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly byte[] _secret;

        public ApplicationAuthManager(
            ApplicationUserManager userManager, 
            IApplicationAuthStore authStore, 
            JwtTokenConfig jwtTokenConfig, 
            TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _authStore = authStore;
            _jwtTokenConfig = jwtTokenConfig;
            _tokenValidationParameters = tokenValidationParameters;
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }

        public async Task<JwtAuthResult> GenerateTokensAsync(Guid userId, Guid clientId)
        {
            if (GuidHelper.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (GuidHelper.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            // Find user by id
            IUser user = await _userManager.FindByIdAsync(userId.ToString());

            // Get user roles 
            IList<string> roles = await _userManager.GetRolesAsync(user);

            // Set user claims
            IList<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.NormalizedUserName),
                new Claim(ClaimTypes.Email, user.NormalizedEmail)
            };
            AddRolesToClaims(claims, roles);

            // Find client by name
            IClient client = await _authStore.FindClientByIdAsync(clientId);

            // Set jwt security token
            // Web API has a role of the authentication server - audience and issuer are set in JWT token. 
            // Issuer will be checked by the client (web application), and audience will be checked by the resource server (in this application: Web API). 
            JwtSecurityToken jwtToken = new JwtSecurityToken
            (
                issuer: _jwtTokenConfig.Issuer,
                audience: _jwtTokenConfig.Audience,
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
                RefreshToken = refreshToken.Value,
                Roles = roles
            };
        }

        public async Task<JwtAuthResult> RefreshTokensAsync(string refreshToken, string accessToken, Guid clientId)
        {
            (ClaimsPrincipal claimsPrincipal, JwtSecurityToken jwtToken) = await DecodeJwtTokenAsync(accessToken);

            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                throw new SecurityTokenException("Invalid token.");
            }

            IUserRefreshToken dbRefreshToken = await _authStore.FindRefreshTokenByValueAsync(refreshToken);
            if (dbRefreshToken == null)
            {
                throw new SecurityTokenException("Invalid token.");
            }
            if (dbRefreshToken.ClientId != clientId)
            {
                throw new SecurityTokenException("Invalid client.");
            }

            string userName = claimsPrincipal.Identity.Name;
            IUser user = await _userManager.FindByIdAsync(dbRefreshToken.UserId.ToString());

            if (user.NormalizedUserName != userName || dbRefreshToken.DateExpiresUtc < DateTime.UtcNow)
            {
                throw new SecurityTokenException("Invalid token.");
            }

            return await GenerateTokensAsync(user.Id, clientId);
        }

        public Task<(ClaimsPrincipal, JwtSecurityToken)> DecodeJwtTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token.");
            }

            ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);

            return Task.FromResult((claimsPrincipal, validatedToken as JwtSecurityToken));
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

        private void AddRolesToClaims(IList<Claim> claims, IEnumerable<string> roles)
        {
            foreach (string role in roles)
            {
                Claim roleClaim = new Claim(ClaimTypes.Role, role);
                claims.Add(roleClaim);
            }
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