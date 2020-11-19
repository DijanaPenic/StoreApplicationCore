using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using Store.WebAPI.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Controllers
{
    // NoAction on some methods -> Fixed exception on startup - application couldn't resolve IdentityControllerBase authManager and Logger instances 
    // as their types are complex, so needed to ignore some methods.
    abstract public class IdentityControllerBase : ApplicationControllerBase
    {
        private readonly ApplicationUserManager _userManager;

        public IdentityControllerBase(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        [NonAction]
        public IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            return BadRequest(result.Errors);
        }       

        [NonAction]
        public async Task<IUser> GetLoggedInUserAsync()
        {
            string userName = User.FindFirst(ClaimTypes.Name).Value;
            IUser user = await _userManager.FindByNameAsync(userName);

            return user;
        }
    }
}