using System;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using Store.WebAPI.Models;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Identity
{
    public class JwtAuthManager : IJwtAuthManager
    {
        private readonly ApplicationUserManager _userManager;
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly byte[] _secret;

        public JwtAuthManager(ApplicationUserManager userManager, JwtTokenConfig jwtTokenConfig)
        {
            _userManager = userManager;
            _jwtTokenConfig = jwtTokenConfig;
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }

        public async Task<JwtAuthResult> GenerateTokensAsync(string userName, Claim[] claims)
        {
            JwtSecurityToken jwtToken = new JwtSecurityToken
            (
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature)
            );

            // Set access token
            string accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            // Set refresh token
            RefreshToken refreshToken = new RefreshToken
            {
                UserName = userName,
                Value = GenerateRefreshTokenValue(),
                DateExpiresUtc = DateTime.UtcNow.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration)
            };

            //// Find user by username
            //IUser user = await _userManager.FindByNameAsync(userName);

            //// Delete the existing refresh token from the database (if found)
            //IdentityResult removeResult = await RemoveRefreshTokenAsync(user);
            //if (!removeResult.Succeeded) return null;

            //// Save refresh token in the database
            //IdentityResult createResult = await _userManager.SetAuthenticationTokenAsync
            //(
            //    user, 
            //    ApplicationUserManager.ApplicationAuthenticationTokenProvider, 
            //    ApplicationUserManager.RefreshTokenPurpose, 
            //    refreshToken.Value, 
            //    refreshToken.DateExpiresUtc
            //);
            //if (!createResult.Succeeded) return null;

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        //public Task<IdentityResult> RemoveRefreshTokenAsync(IUser user)
        //{
        //    return _userManager.RemoveAuthenticationTokenAsync
        //    (
        //        user, 
        //        ApplicationUserManager.ApplicationAuthenticationTokenProvider,
        //        ApplicationUserManager.RefreshTokenPurpose
        //    );
        //}

        //public Task RemoveExpiredRefreshTokensAsync()
        //{
        //    return _userManager.RemoveExpiredAuthenticationTokensAsync
        //    (
        //        ApplicationUserManager.ApplicationAuthenticationTokenProvider, 
        //        ApplicationUserManager.RefreshTokenPurpose
        //    );
        //}

        private string GenerateRefreshTokenValue()
        {
            byte[] randomNumber = new byte[32];
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }

    public interface IJwtAuthManager
    {
        //Task<JwtAuthResult> GenerateTokensAsync(string userName, Claim[] claims);

        //Task<IdentityResult> RemoveRefreshTokenAsync(IUser user);

        //Task RemoveExpiredRefreshTokensAsync();

        //JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now);

        //(ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
    }

    public class JwtAuthResult
    {
        public string AccessToken { get; set; }

        public RefreshToken RefreshToken { get; set; }
    }

    public class RefreshToken
    {
        public string UserName { get; set; }    

        public string Value { get; set; }

        public DateTime DateExpiresUtc { get; set; }
    }
}