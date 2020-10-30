using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Store.WebAPI.Controllers
{
    public class ExtendedControllerBase : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult InternalServerError()
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Created()
        {
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}