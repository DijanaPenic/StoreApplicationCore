using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Store.WebAPI.Models;
using Store.WebAPI.Identity;
using Store.Models.Api.Identity;
using Store.Model.Common.Models.Identity;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Store.WebAPI.Controllers
{
    abstract public class IdentityControllerBase : ExtendedControllerBase
    {
        protected IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            return BadRequest(result.Errors);
        }

        protected async Task<IActionResult> AuthenticateAsync(ApplicationAuthManager authManager, ILogger logger, SignInResult signInResult, IUser user, Guid clientId)
        {
            AuthenticateResponseApiModel authenticationResponse = new AuthenticateResponseApiModel
            {
                UserId = user.Id,
                RequiresTwoFactor = signInResult.RequiresTwoFactor
            };

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    return Unauthorized($"User [{user.UserName}] has been locked out.");
                }
                if (signInResult.IsNotAllowed)
                {
                    return Unauthorized($"User [{user.UserName}] is not allowed to log in.");
                }
                if (signInResult.RequiresTwoFactor)
                {
                    return Ok(authenticationResponse);
                }

                return Unauthorized($"Failed to log in [{user.UserName}].");
            }

            logger.LogInformation($"User [{user.UserName}] has logged in the system.");

            JwtAuthResult jwtResult = await authManager.GenerateTokensAsync(user.Id, clientId);

            authenticationResponse.Roles = jwtResult.Roles.ToArray();
            authenticationResponse.AccessToken = jwtResult.AccessToken;
            authenticationResponse.RefreshToken = jwtResult.RefreshToken;

            if (signInResult.Succeeded)
            {
                // Need to delete the "identity" cookie for the authorized user - created by SignInManager 
                Response.Cookies.Delete(".AspNetCore.Identity.Application");
                Response.Headers.Remove("Set-Cookie");
            }

            return Ok(authenticationResponse);
        }
    }
}