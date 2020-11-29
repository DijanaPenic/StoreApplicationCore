namespace Store.Service.Options
{
    public class JwtTokenOptions
    {
        public const string Position = "JwtToken";

        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
    }
}