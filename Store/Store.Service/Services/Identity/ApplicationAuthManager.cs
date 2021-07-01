using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Service.Options;
using Store.Service.Constants;
using Store.Service.Common.Services.Identity;
using Store.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Repositories.Identity.Stores;

namespace Store.Services.Identity
{
    public class ApplicationAuthManager
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IApplicationAuthStore _authStore;
        private readonly JwtTokenOptions _jwtTokenConfig;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public ApplicationAuthManager(
            ApplicationUserManager userManager, 
            IApplicationAuthStore authStore,
            IOptions<JwtTokenOptions> jwtTokenOptions,
            TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _authStore = authStore;
            _jwtTokenConfig = jwtTokenOptions.Value;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<IJwtAuthResult> GenerateTokensAsync(Guid userId, Guid clientId, string externalLoginProvider = null)
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
                new(ClaimTypes.Name, user.NormalizedUserName),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ApplicationClaimTypes.ClientIdentifier, clientId.ToString())
            };

            // Add the AuthenticationMethod claim to the user so that we can find the provider the user used to sign in to the app.
            if (!string.IsNullOrEmpty(externalLoginProvider))
            {
                claims.Add(new Claim(ClaimTypes.AuthenticationMethod, externalLoginProvider));
            }

            AddRolesToClaims(claims, roles);

            // Find client by name
            IClient client = await _authStore.FindClientByKeyAsync(clientId);

            // Set jwt security token
            // Web API has a role of the authentication server - audience and issuer need to be set in JWT token. 
            // Issuer will be checked by the client (web application), and audience will be checked by the resource server (in this application: Web API). 
            byte[] secret = Encoding.ASCII.GetBytes(_jwtTokenConfig.Secret);

            JwtSecurityToken jwtToken = new(
                issuer: _jwtTokenConfig.Issuer,
                audience: _jwtTokenConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(client.AccessTokenLifeTime),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
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
            IUserRefreshTokenKey userRefreshTokenKey = new UserRefreshTokenKey() 
            { 
                ClientId = client.Id,
                UserId = user.Id
            };
            await _authStore.RemoveRefreshTokenByKeyAsync(userRefreshTokenKey);

            // Save refresh token in the database
            await _authStore.AddRefreshTokenAsync(refreshToken);

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Value,
                Roles = roles
            };
        }

        public async Task<IJwtAuthResult> RenewTokensAsync(string refreshToken, string accessToken, Guid clientId)
        {
            (ClaimsPrincipal claimsPrincipal, JwtSecurityToken jwtToken) = await DecodeAccessTokenAsync(accessToken);

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

            // Retrieve provider information
            Claim authMethodClaim = claimsPrincipal.Claims.Where(c => c.Type == ClaimTypes.AuthenticationMethod).FirstOrDefault();
            string provider = authMethodClaim?.Value;

            return await GenerateTokensAsync(user.Id, clientId, provider);
        }

        public async Task<ClaimsPrincipal> ValidateAccessTokenAsync(string token)
        {
            (ClaimsPrincipal claimsPrincipal, JwtSecurityToken jwtToken) = await DecodeAccessTokenAsync(token);

            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                return default;
            }

            return claimsPrincipal;
        }

        private Task<(ClaimsPrincipal, JwtSecurityToken)> DecodeAccessTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token.");
            }

            ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);

            return Task.FromResult((claimsPrincipal, validatedToken as JwtSecurityToken));
        }

        public Task RemoveExpiredRefreshTokensAsync()
        {
            return _authStore.RemoveExpiredRefreshTokensAsync();
        }

        public Task<IClient> GetClientByKeyAsync(Guid key)
        {
            return _authStore.FindClientByKeyAsync(key);
        }

        public async Task<string> AuthenticateClientAsync(Guid clientId, string clientSecret)
        {
            if (GuidHelper.IsNullOrEmpty(clientId))
            {
                return "Client is required.";
            }

            IClient client = await _authStore.FindClientByKeyAsync(clientId);

            if(client == null)
            {
                return $"Client '{clientId}' is not registered in the system.";
            }
            if(!client.Active)
            {
                return $"Client '{clientId}' is not active.";
            }
            if (client.ApplicationType == ApplicationType.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    return $"Client secret should be provided for client '{clientId}'";
                }
                else if (client.Secret != HashHelper.GetSHA512Hash(clientSecret))
                {
                    return $"Client secret is not valid for client '{clientId}'";
                }
            }

            return string.Empty;
        }

        public async Task<string> ValidateClientUrlAsync(Guid clientId, string url)
        {
            if (GuidHelper.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            bool validUrl = Uri.TryCreate(url, UriKind.Absolute, out Uri redirectUri);
            if (!validUrl)
            {
                return $"URL '{url}' is not valid.";
            }

            IClient client = await _authStore.FindClientByKeyAsync(clientId);
            if (client == null)
            {
                return $"Client '{clientId}' is not registered in the system.";
            }

            if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            {
                return $"The given URL is not allowed by Client '{clientId}' configuration.";
            }

            return string.Empty;
        }

        private static void AddRolesToClaims(IList<Claim> claims, IEnumerable<string> roles)
        {
            foreach (string role in roles)
            {
                Claim roleClaim = new Claim(ClaimTypes.Role, role);
                claims.Add(roleClaim);
            }
        }

        private static string GenerateRefreshTokenValue()
        {
            byte[] randomNumber = new byte[32];
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}