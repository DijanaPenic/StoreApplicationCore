namespace Store.WebAPI.Models.Settings
{
    public class CountryGetApiModel
    {
        public string Name { get; set; }

        public string AlphaTwoCode { get; set; }

        public string AlphaThreeCode { get; set; }

        public string[] CallingCodes { get; set; }

        public string Region { get; set; }

        public string NumericCode { get; set; }
    }
}