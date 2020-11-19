using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Store.WebAPI.Controllers
{
    // NoAction on some methods -> Fixed exception on startup - application couldn't resolve IdentityControllerBase authManager and Logger instances 
    // as their types are complex, so needed to ignore some methods.
    abstract public class IdentityControllerBase : ApplicationControllerBase
    {
        [NonAction]
        public IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            return BadRequest(result.Errors);
        }       
    }
}