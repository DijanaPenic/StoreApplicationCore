namespace Store.Models.Api.Identity
{
    public class RemoveLoginViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}