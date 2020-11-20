namespace Store.Models.Api.Identity
{
    public class AuthenticateInfoGetApiModel
    {
        public bool IsAuthenticated { get; set; }

        public string Username { get; set; }

        public string AuthenticationMethod { get; set; }

        public bool DisplaySetPassword { get; set; }
    }
}