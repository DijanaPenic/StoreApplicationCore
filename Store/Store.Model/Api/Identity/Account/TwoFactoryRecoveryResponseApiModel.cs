using System.Text.Json.Serialization;

namespace Store.Models.Api.Identity
{
    public class TwoFactoryRecoveryResponseApiModel
    {
        [JsonPropertyName("recovery_codes")]
        public string[] RecoveryCodes { get; set; }
    }
}