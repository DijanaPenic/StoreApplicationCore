using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class PhoneNumberVerifyPostApiModel
    {
        [Required]
        public string IsoCountryCode { get; set; }

        [Required]
        public string CountryCodeNumber { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public bool IsVoiceCall { get; set; }
    }
}