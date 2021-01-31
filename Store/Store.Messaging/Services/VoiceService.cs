using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Options;

using Store.Messaging.Options;
using Store.Messaging.Services.Common;

namespace Store.Messaging.Services
{
    public class VoiceService : IVoiceService
    {
        private readonly TwilioAuthOptions _config; 
        
        public VoiceService(IOptions<TwilioAuthOptions> options)
        {
            _config = options.Value;

            TwilioClient.Init(_config.AccountSID, _config.AuthToken);
        }

        public Task CallAsync(string phoneNumber, Uri url)
        {
            Task<CallResource> message = CallResource.CreateAsync(
                from: new PhoneNumber(_config.FromPhoneNumber),
                to: new PhoneNumber(phoneNumber),
                url: url
            );

            return message;
        }
    }
}