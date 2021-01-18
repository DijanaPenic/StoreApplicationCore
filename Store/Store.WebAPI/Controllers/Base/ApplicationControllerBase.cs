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

        protected bool IsCurrentUser(Guid userId) => GetCurrentUserId() == userId;

        protected Guid GetCurrentUserId() => Guid.Parse(User.Claims.FirstOrDefault(uc => uc.Type == ClaimTypes.NameIdentifier)?.Value);

        protected Guid GetCurrentUserClientId() => Guid.Parse(User.Claims.FirstOrDefault(uc => uc.Type == ApplicationClaimTypes.ClientIdentifier)?.Value);
    }
}