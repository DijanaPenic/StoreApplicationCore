namespace Store.WebAPI.Infrastructure.Models
{
    public class AuthMessageSenderOptions
    {
        public const string Position = "EmailServer";

        public string SendGridUser { get; set; }

        public string SendGridKey { get; set; }

        public string FromEmail { get; set; }
    }
}