using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api
{
    public class GoogleReCaptchaModelApiBase
    {
        [Required]
        [GoogleReCaptchaValidationAttribute]
        public string GoogleReCaptchaResponse { get; set; }
    }
}