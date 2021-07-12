namespace Store.Messaging.Options
{
    public class SendGridAuthOptions
    {
        public const string Position = "SendGrid";

        public string User { get; set; }

        public string ApiKey { get; set; }

        public string FromEmail { get; set; }
    }
}