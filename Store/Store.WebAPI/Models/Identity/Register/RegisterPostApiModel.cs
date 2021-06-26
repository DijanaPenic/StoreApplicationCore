using FluentValidation;
using Microsoft.Extensions.Configuration;

using Store.WebAPI.Models.Extensions;

namespace Store.WebAPI.Models.Identity
{
    public class RegisterPostApiModel : GoogleReCaptchaModelApiBase
    {
        public string UserName { get; set; }
        
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string ActivationUrl { get; set; }
    }
    
    public class RegisterPostApiModelValidator : GoogleReCaptchaModelApiBaseValidator<RegisterPostApiModel>
    {
        public RegisterPostApiModelValidator(IConfiguration configuration) : base(configuration)
        {
            RuleFor(register => register.UserName).NotEmpty();
            RuleFor(register => register.Email).NotEmpty().EmailAddress();
            RuleFor(register => register.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(register => register.LastName).NotEmpty().MaximumLength(50);
            RuleFor(register => register.Password).NotEmpty().Password();
        
            RuleFor(register => register.ConfirmPassword)
                .NotEmpty()
                .Equal(register => register.Password).WithMessage("The new password and confirmation password must match.");
            
            RuleFor(register => register.ActivationUrl).NotEmpty();
        }
    }
}