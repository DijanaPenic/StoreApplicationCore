using System.Net;
using System.Net.Http;
using System.Text.Json;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace Store.WebAPI.Models
{
    public class GoogleReCaptchaModelApiBase
    {
        public string GoogleCaptcha { get; set; }
    }
    
    public abstract class GoogleReCaptchaModelApiBaseValidator<T> : AbstractValidator<T> where T : GoogleReCaptchaModelApiBase
    {
        private readonly IConfiguration _configuration;
        
        protected GoogleReCaptchaModelApiBaseValidator(IConfiguration configuration)
        {
            _configuration = configuration;

            RuleFor(gc => gc.GoogleCaptcha)
                .NotEmpty()
                .Custom(IsCaptchaValid);
        }

        private void IsCaptchaValid(string value, ValidationContext<T> validationContext)
        {
            const string errorMessage = "'Google Captcha' is not valid.";

            HttpClient httpClient = new HttpClient();
            string reCaptchaSecret = _configuration.GetValue<string>("GoogleReCaptcha:SecretKey");
            
            HttpResponseMessage httpResponse = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={reCaptchaSecret}&response={value}").Result;
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                validationContext.AddFailure(errorMessage);
            }

            string jsonResponse = httpResponse.Content.ReadAsStringAsync().Result;
            JsonElement jsonData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            if (jsonData.GetProperty("success").GetBoolean() != true)
            {
                validationContext.AddFailure(errorMessage);
            }
        }
    }
}