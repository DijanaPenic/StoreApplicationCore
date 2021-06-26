using FluentValidation;

using Store.WebAPI.Models.Extensions;

namespace Store.WebAPI.Models.Identity
{
    public class ChangePasswordPatchApiModel
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public bool SendMailNotification { get; set; }
    }

    public class ChangePasswordPatchApiModelValidator : AbstractValidator<ChangePasswordPatchApiModel>
    {
        public ChangePasswordPatchApiModelValidator()
        {
            RuleFor(changePassword => changePassword.OldPassword);
            RuleFor(changePassword => changePassword.NewPassword).NotEmpty().Password();

            RuleFor(changePassword => changePassword.ConfirmPassword)
                .NotEmpty()
                .Equal(changePassword => changePassword.NewPassword).WithMessage("The new password and confirmation password must match.");
        }
    }
}