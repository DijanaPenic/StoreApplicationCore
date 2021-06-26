using FluentValidation;

namespace Store.WebAPI.Models.Identity
{
    public class AuthenticatePasswordPostApiModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class AuthenticatePasswordPostApiModelValidator : AbstractValidator<AuthenticatePasswordPostApiModel>
    {
        public AuthenticatePasswordPostApiModelValidator()
        {
            RuleFor(authPassword => authPassword.UserName).NotEmpty();
            RuleFor(authPassword => authPassword.Password).NotEmpty();
        }
    }
}