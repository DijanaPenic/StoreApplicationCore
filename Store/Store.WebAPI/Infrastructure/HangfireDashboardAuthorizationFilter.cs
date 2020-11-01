using System;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Store.Common.Helpers.Identity;

namespace Store.WebAPI.Infrastructure
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private const string HangFireCookieName = "HangfireCookie";
        private const int CookieExpirationMinutes = 60;
        private readonly ILogger _logger;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public HangfireDashboardAuthorizationFilter(TokenValidationParameters tokenValidationParameters, ILogger<HangfireDashboardAuthorizationFilter> logger)
        {
            _tokenValidationParameters = tokenValidationParameters;
            _logger = logger;
        }

        public bool Authorize(DashboardContext context)
        {
#if DEBUG
            return true;
#else
            HttpContext httpContext = context.GetHttpContext();

            bool setCookie = false;
            string access_token;

            // Try to get token from query string
            if (httpContext.Request.Query.ContainsKey("access_token"))
            {
                access_token = httpContext.Request.Query["access_token"].FirstOrDefault();
                setCookie = true;
            }
            else
            {
                access_token = httpContext.Request.Cookies[HangFireCookieName];
            }

            if (string.IsNullOrEmpty(access_token))
            {
                return false;
            }

            try
            {
                JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
                ClaimsPrincipal claims = jwtHandler.ValidateToken(access_token, _tokenValidationParameters, out SecurityToken validatedToken);

                if (!claims.IsInRole(RoleHelper.Admin))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during dashboard hangfire jwt validation process.");

                throw ex;
            }

            if (setCookie)
            {
                httpContext.Response.Cookies.Append(HangFireCookieName,
                access_token,
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddHours(CookieExpirationMinutes)
                });
            }

            return true;
#endif
        }
    }
}