namespace Store.Models.Api.Identity
{
    public class RenewTokenResponseApiModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}