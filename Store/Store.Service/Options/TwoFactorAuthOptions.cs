namespace Store.Service.Options
{
    public class TwoFactorAuthOptions
    {
        public const string Position = "TwoFactorAuthentication";

        public bool EncryptionEnabled { get; set; }

        public string EncryptionKey { get; set; }
    }
}