using System;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;

using Store.Common.Helpers;
using Store.Services.Identity;
using Store.Service.Constants;

namespace Store.WebAPI.Infrastructure.Authentication.Handlers
{
    // Blog Post: https://www.roundthecode.com/dotnet/how-to-add-basic-authentication-to-asp-net-core-application
    public class ClientAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> 
    {
        private readonly ApplicationAuthManager _authManager;

        public ClientAuthenticationHandler
        (
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ApplicationAuthManager authManager
        )
        : base(options, logger, encoder, clock)
        {
            _authManager = authManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Response.Headers.Add("WWW-Authenticate", "Basic");

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Authorization header missing.");
            }

            // Get authorization key
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            Regex authHeaderRegex = new Regex(@"Basic (.*)");

            if (!authHeaderRegex.IsMatch(authorizationHeader))
            {
                return AuthenticateResult.Fail("Authorization code not formatted properly.");
            }

            string authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderRegex.Replace(authorizationHeader, "$1"))); 
            string[] authSplit = authBase64.Split(':', 2);
            string authClientId = authSplit[0];
            string authClientSecret = authSplit.Length > 1 ? authSplit[1] : string.Empty;

            if (!Guid.TryParse(authClientId, out Guid clientId) || GuidHelper.IsNullOrEmpty(clientId))
            {
                return AuthenticateResult.Fail($"Client '{clientId}' format is invalid.");
            }

            string clientAuthResult = await _authManager.AuthenticateClientAsync(clientId, authClientSecret);
            if (!string.IsNullOrEmpty(clientAuthResult))
            {
                return AuthenticateResult.Fail(clientAuthResult);
            }

            ClaimsIdentity identity = new ClaimsIdentity("ClientAuthentication");
            identity.AddClaim(new Claim(ApplicationClaimTypes.ClientIdentifier, authClientId));

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
        }
    }
}