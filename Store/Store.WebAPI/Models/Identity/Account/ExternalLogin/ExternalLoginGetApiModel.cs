namespace Store.WebAPI.Models.Identity
{
    public class ExternalLoginGetApiModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public string ProviderDisplayName { get; set; }
    }
}