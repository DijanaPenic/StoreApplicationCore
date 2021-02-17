namespace Store.WebAPI.Models.Identity
{
    public class RenewTokenResponseApiModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}