using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Store.WebAPI.Controllers
{
    abstract public class ExtendedControllerBase : ControllerBase
    {
        protected IActionResult InternalServerError()
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        protected IActionResult Created()
        {
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}