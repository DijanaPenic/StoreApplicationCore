namespace Store.WebAPI.Infrastructure.Models
{
    public class SmsSenderAuthOptions
    {
        public const string Position = "Twilio";

        public string AccountSID { get; set; }

        public string AuthToken { get; set; }

        public string FromPhoneNumber { get; set; }
    }
}