using FluentValidation;

namespace Store.WebAPI.Models.Identity
{
    public class AuthenticateTwoFactorPostApiModel
    {
        public string Code { get; set; }

        public bool UseRecoveryCode { get; set; }
    }

    public class AuthenticateTwoFactorPostApiModelValidator : AbstractValidator<AuthenticateTwoFactorPostApiModel>
    {
        public AuthenticateTwoFactorPostApiModelValidator()
        {
            RuleFor(authTwFactor => authTwFactor.Code).NotEmpty().Length(6, 6);
        }
    }
}