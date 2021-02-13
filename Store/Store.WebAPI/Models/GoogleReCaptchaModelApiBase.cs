using System.ComponentModel.DataAnnotations;

using Store.WebAPI.Infrastructure.Validation.Attributes;

namespace Store.WebAPI.Models
{
    public class GoogleReCaptchaModelApiBase
    {
        [Required]
        [GoogleReCaptchaValidation]
        public string GoogleReCaptchaResponse { get; set; }
    }
}