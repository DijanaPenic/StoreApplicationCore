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
            RuleFor(r => r.UserName).NotEmpty();
            RuleFor(r => r.Email).NotEmpty().EmailAddress();
            RuleFor(r => r.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(r => r.LastName).NotEmpty().MaximumLength(50);
            RuleFor(r => r.Password).NotEmpty().Password();
        
            RuleFor(r => r.ConfirmPassword)
                .NotEmpty()
                .Equal(r => r.Password).WithMessage("The new password and confirmation password must match.");
            
            RuleFor(r => r.ActivationUrl).NotEmpty();
        }
    }
}