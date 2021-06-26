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
            RuleFor(authExternal => authExternal.ConfirmationUrl).NotEmpty();
        }
    }
}