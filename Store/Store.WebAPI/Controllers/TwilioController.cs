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
        public TwilioController()
        {

        }

        [HttpGet]
        [Route("phone-number/token/{token}", Name = RouteNames.TwilioPhoneNumberVerificationToken)]
        public IActionResult PhoneNumberVerificationToken([FromRoute] string token)
        {
            VoiceResponse voiceResponse = new VoiceResponse();
            voiceResponse.Say($"Your phone number verification token is {token}", VoiceEnum.Alice, 3);

            return Ok(voiceResponse.ToString());
        }
    }
}