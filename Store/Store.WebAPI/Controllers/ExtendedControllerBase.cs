using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Store.WebAPI.Controllers
{
    public class ExtendedControllerBase : ControllerBase
    {
        public IActionResult InternalServerError()
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Created()
        {
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}