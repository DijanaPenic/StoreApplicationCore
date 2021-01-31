using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using Store.Messaging.Options;
using Store.Messaging.Services.Common;

namespace Store.Messaging.Services
{
    public class SmsService : ISmsService
    {
        private readonly TwilioAuthOptions _config; 
        
        public SmsService(IOptions<TwilioAuthOptions> options)
        {
            _config = options.Value;

            TwilioClient.Init(_config.AccountSID, _config.AuthToken);
        }

        public Task SendSmsAsync(string phoneNumber, string body)
        {         
            Task<MessageResource> message = MessageResource.CreateAsync(
                body: body,
                from: new PhoneNumber(_config.FromPhoneNumber),
                to: new PhoneNumber(phoneNumber)
            );

            return message;
        }
    }
}