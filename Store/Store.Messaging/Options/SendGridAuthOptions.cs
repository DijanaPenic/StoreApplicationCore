namespace Store.Messaging.Options
{
    public class SendGridAuthOptions
    {
        public const string Position = "SendGrid";

        public string ApiKeyValue { get; set; }

        public string FromEmail { get; set; }
    }
}