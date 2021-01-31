namespace Store.Messaging.Options
{
    public class SendGridAuthOptions
    {
        public const string Position = "SendGrid";

        public string User { get; set; }

        public string Key { get; set; }

        public string FromEmail { get; set; }
    }
}