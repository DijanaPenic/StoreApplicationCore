namespace Store.Messaging.Templates.Models.Email
{
    public class ConfirmExternalAccountViewModel
    {
        public ConfirmExternalAccountViewModel(string confirmUrl, string providerDisplayName)
        {
            ConfirmUrl = confirmUrl;
            ProviderDisplayName = providerDisplayName;
        }

        public string ConfirmUrl { get; set; }

        public string ProviderDisplayName { get; set; }
    }
}