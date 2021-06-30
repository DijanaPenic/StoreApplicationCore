namespace Store.WebAPI.Models.Identity
{
    public class RenewTokenGetApiModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}