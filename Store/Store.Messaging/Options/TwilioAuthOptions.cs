namespace Store.Messaging.Options
{
    public class TwilioAuthOptions
    {
        public const string Position = "Twilio";

        public string AccountSID { get; set; }

        public string AuthToken { get; set; }

        public string FromPhoneNumber { get; set; }
    }
}