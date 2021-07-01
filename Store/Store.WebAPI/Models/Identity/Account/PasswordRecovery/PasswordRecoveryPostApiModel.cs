using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace Store.WebAPI.Models.Identity
{
    public class PasswordRecoveryPostApiModel : GoogleReCaptchaModelApiBase
    {
        public string Email { get; set; }

        public string ConfirmationUrl { get; set; }
    }

    public class PasswordRecoveryPostApiModelValidator : GoogleReCaptchaModelApiBaseValidator<PasswordRecoveryPostApiModel>
    {
        public PasswordRecoveryPostApiModelValidator(IConfiguration configuration) : base(configuration)
        {
            RuleFor(pr => pr.Email).NotEmpty().EmailAddress();
            RuleFor(pr => pr.ConfirmationUrl).NotEmpty();
        }
    }
}