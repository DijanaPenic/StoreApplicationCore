namespace Store.WebAPI.Infrastructure.Models
{
    public class EmailSenderAuthOptions
    {
        public const string Position = "SendGrid";

        public string User { get; set; }

        public string Key { get; set; }

        public string FromEmail { get; set; }
    }
}