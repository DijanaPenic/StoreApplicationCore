using FluentValidation;

using Store.WebAPI.Models.Extensions;

namespace Store.WebAPI.Models.Identity
{
    public class PasswordRecoveryPatchApiModel
    {
        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }

        public string PasswordRecoveryToken { get; set; }
    }

    public class PasswordRecoveryPatchApiModelValidator : AbstractValidator<PasswordRecoveryPatchApiModel>
    {
        public PasswordRecoveryPatchApiModelValidator()
        {
            RuleFor(passwordRecovery => passwordRecovery.NewPassword).NotEmpty().Password();

            RuleFor(passwordRecovery => passwordRecovery.ConfirmNewPassword)
                .NotEmpty()
                .Equal(passwordRecovery => passwordRecovery.NewPassword).WithMessage("The new password and confirmation password must match.");

            RuleFor(passwordRecovery => passwordRecovery.PasswordRecoveryToken).NotEmpty();
        }
    }
}