namespace Store.Models.Api.Identity
{
    public class AuthenticatorKeyGetApiModel
    {
        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }
    }
}