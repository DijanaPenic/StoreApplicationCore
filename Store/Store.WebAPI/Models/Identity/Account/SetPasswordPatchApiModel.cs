using FluentValidation;

using Store.WebAPI.Models.Extensions;

namespace Store.WebAPI.Models.Identity
{
    public class SetPasswordPatchApiModel
    {
        public string Password { get; set; }
    }

    public class SetPasswordPatchApiModelValidator : AbstractValidator<SetPasswordPatchApiModel>
    {
        public SetPasswordPatchApiModelValidator()
        {
            RuleFor(setPassword => setPassword.Password).NotEmpty().Password();
        }
    }
}