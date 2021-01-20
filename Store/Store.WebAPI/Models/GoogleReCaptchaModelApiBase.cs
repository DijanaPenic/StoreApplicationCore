using System.ComponentModel.DataAnnotations;

using Store.WebAPI.Infrastructure.Authorization.Attributes;

namespace Store.WebAPI.Models
{
    public class GoogleReCaptchaModelApiBase
    {
        [Required]
        [GoogleReCaptchaValidation]
        public string GoogleReCaptchaResponse { get; set; }
    }
}