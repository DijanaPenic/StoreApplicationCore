using Twilio.TwiML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using static Twilio.TwiML.Voice.Say;

using Store.WebAPI.Constants;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/twilio")]
    public class TwilioController : ControllerBase
    {
        [HttpGet]
        [Route("calls/token", Name = RouteNames.TwilioPhoneNumberVerificationToken)]
        public IActionResult PhoneNumberVerificationToken([FromQuery] string code)
        {
            VoiceResponse voiceResponse = new();
            voiceResponse.Say($"Your phone number verification token is {code}", VoiceEnum.Alice, 3);

            return Ok(voiceResponse.ToString());
        }
    }
}