using FluentValidation;

namespace Store.WebAPI.Models.Identity
{
    public class AuthenticateExternalPostApiModel
    {
        public string ConfirmationUrl { get; set; }
    }

    public class AuthenticateExternalPostApiModelValidator : AbstractValidator<AuthenticateExternalPostApiModel>
    {
        public AuthenticateExternalPostApiModelValidator()
        {
            RuleFor(ae => ae.ConfirmationUrl).NotEmpty();
        }
    }
}