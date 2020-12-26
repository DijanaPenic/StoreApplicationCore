namespace Store.Messaging.Templates.Models.Email
{
    public class ConfirmExternalAccountEmailViewModel
    {
        public ConfirmExternalAccountEmailViewModel(string confirmUrl, string providerDisplayName)
        {
            ConfirmUrl = confirmUrl;
            ProviderDisplayName = providerDisplayName;
        }

        public string ConfirmUrl { get; set; }

        public string ProviderDisplayName { get; set; }
    }
}