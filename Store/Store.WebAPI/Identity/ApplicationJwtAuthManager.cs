using System;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

using Store.WebAPI.Models;
using Store.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Service.Common.Services.Identity;

namespace Store.WebAPI.Identity
{
    public class ApplicationJwtAuthManager
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IApplicationAuthStore _authStore;
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly byte[] _secret;

        public ApplicationJwtAuthManager(ApplicationUserManager userManager, IApplicationAuthStore authStore, JwtTokenConfig jwtTokenConfig)
        {
            _userManager = userManager;
            _authStore = authStore;
            _jwtTokenConfig = jwtTokenConfig;
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }

        public async Task<JwtAuthResult> GenerateTokensAsync(string userName, string clientName, Claim[] claims)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName));

            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentNullException(nameof(clientName));

            // Find user by username
            IUser user = await _userManager.FindByNameAsync(userName);

            // Find client by name
            IClient client = await _authStore.FindClientByNameAsync(clientName);

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
                RefreshToken = refreshToken
            };
        }

        public Task RemoveRefreshTokenAsync(Guid userId, Guid clientId)
        {
            return _authStore.RemoveRefreshTokenAsync(userId, clientId);
        }

        public Task RemoveExpiredRefreshTokensAsync()
        {
            return _authStore.RemoveExpiredRefreshTokensAsync();
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