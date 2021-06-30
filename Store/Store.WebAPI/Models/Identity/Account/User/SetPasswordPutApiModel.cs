using FluentValidation;

using Store.WebAPI.Models.Extensions;

namespace Store.WebAPI.Models.Identity
{
    public class SetPasswordPutApiModel
    {
        public string Password { get; set; }
    }

    public class SetPasswordPutApiModelValidator : AbstractValidator<SetPasswordPutApiModel>
    {
        public SetPasswordPutApiModelValidator()
        {
            RuleFor(setPassword => setPassword.Password).NotEmpty().Password();
        }
    }
}