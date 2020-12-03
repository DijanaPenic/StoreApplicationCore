using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;

namespace Store.WebAPI.Controllers
{
    // NoAction on some methods -> Fixed exception on startup - application couldn't resolve IdentityControllerBase authManager and Logger instances 
    // as their types are complex, so needed to ignore some methods.
    abstract public class IdentityControllerBase : ApplicationControllerBase
    {
        [NonAction]
        protected IActionResult GetErrorResult(IdentityResult result)
        {
            return BadRequest(result.Errors);
        }

        [NonAction]
        protected bool IsCurrentUser(Guid userId)
        {
            return (GetCurrentUserId() == userId);
        }

        [NonAction]
        protected Guid GetCurrentUserId()
        {
            return Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }
    }
}