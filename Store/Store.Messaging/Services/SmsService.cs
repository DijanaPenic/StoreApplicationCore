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
        private readonly TwilioAuthOptions _smsConfig; 
        
        public SmsService(IOptions<TwilioAuthOptions> options)
        {
            _smsConfig = options.Value;

            TwilioClient.Init(_smsConfig.AccountSID, _smsConfig.AuthToken);
        }

        public Task SendSmsAsync(string phoneNumber, string body)
        {         
            Task<MessageResource> message = MessageResource.CreateAsync(
                body: body,
                from: new PhoneNumber(_smsConfig.FromPhoneNumber),
                to: new PhoneNumber(phoneNumber)
            );

            return message;
        }
    }
}