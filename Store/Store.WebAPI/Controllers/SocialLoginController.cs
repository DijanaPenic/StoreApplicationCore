using Microsoft.AspNetCore.Mvc;

// Blog post: https://chsakell.com/2019/07/28/asp-net-core-identity-series-external-provider-authentication-registration-strategy/
namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("social-login")]
    public class SocialLoginController : IdentityControllerBase
    {
    }
}