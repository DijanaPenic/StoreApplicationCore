namespace Store.WebAPI.Models.Identity
{
    public class AuthenticateInfoGetApiModel
    {
        public bool IsAuthenticated { get; set; }

        public string Username { get; set; }

        public string ExternalLoginProvider { get; set; }

        public bool DisplaySetPassword { get; set; }
    }
}