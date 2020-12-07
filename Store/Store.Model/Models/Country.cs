using System.Text.Json.Serialization;

using Store.Model.Common.Models;

namespace Store.Models
{
    public class Country : ICountry
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("alpha2Code")]
        public string AlphaTwoCode { get; set; }

        [JsonPropertyName("alpha3Code")]
        public string AlphaThreeCode { get; set; }

        [JsonPropertyName("callingCodes")]
        public string[] CallingCodes { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("numericCode")]
        public string NumericCode { get; set; }
    }
}