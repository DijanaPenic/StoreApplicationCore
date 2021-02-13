using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace Store.WebAPI.Infrastructure.Validation.Attributes
{
    // Blog post: https://dejanstojanovic.net/aspnet/2018/may/using-google-recaptcha-v2-in-aspnet-core-web-application/

    public class GoogleReCaptchaValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Lazy<ValidationResult> errorResult = new Lazy<ValidationResult>(() 
                => new ValidationResult("Google reCAPTCHA validation failed", new string[] { validationContext.MemberName }));

            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return errorResult.Value;
            }

            IConfiguration configuration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));
            string reCaptchResponse = value.ToString();
            string reCaptchaSecret = configuration.GetValue<string>("GoogleReCaptcha:SecretKey");

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponse = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={reCaptchaSecret}&response={reCaptchResponse}").Result;
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                return errorResult.Value;
            }

            string jsonResponse = httpResponse.Content.ReadAsStringAsync().Result;
            JsonElement jsonData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            if (jsonData.GetProperty("success").GetBoolean() != true)
            {
                return errorResult.Value;
            }

            return ValidationResult.Success;
        }
    }
}