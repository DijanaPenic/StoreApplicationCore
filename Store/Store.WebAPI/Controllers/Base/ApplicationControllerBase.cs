using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Store.Service.Constants;

namespace Store.WebAPI.Controllers
{
    abstract public class ApplicationControllerBase : ControllerBase
    {
        protected IActionResult InternalServerError() => StatusCode(StatusCodes.Status500InternalServerError);

        protected IActionResult Created() => StatusCode(StatusCodes.Status201Created);

        protected bool IsCurrentUser(Guid userId) => GetUserId(User) == userId;

        protected Guid GetCurrentUserClientId() => GetClientId(User);

        protected bool IsUser(Guid userId, ClaimsPrincipal claimsPrincipal) => GetUserId(claimsPrincipal) == userId;

        protected Guid GetUserId(ClaimsPrincipal claimsPrincipal) => Guid.Parse(claimsPrincipal.Claims.FirstOrDefault(uc => uc.Type == ClaimTypes.NameIdentifier)?.Value);

        protected Guid GetClientId(ClaimsPrincipal claimsPrincipal) => Guid.Parse(claimsPrincipal.Claims.FirstOrDefault(uc => uc.Type == ApplicationClaimTypes.ClientIdentifier)?.Value);
        
        protected Uri GetAbsoluteUri(string relativeUrl) => new Uri($"{Request.Scheme}://{Request.Host}{relativeUrl}", UriKind.Absolute);
    }
}