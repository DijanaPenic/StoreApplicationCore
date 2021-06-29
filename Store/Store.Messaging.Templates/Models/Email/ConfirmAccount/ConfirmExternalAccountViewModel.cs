namespace Store.Messaging.Templates.Models.Email
{
    public class ConfirmExternalAccountViewModel
    {
        public ConfirmExternalAccountViewModel(string confirmUrl, string providerDisplayName)
        {
            ConfirmUrl = confirmUrl;
            ProviderDisplayName = providerDisplayName;
        }

        public string ConfirmUrl { get; }

        public string ProviderDisplayName { get; }
    }
}