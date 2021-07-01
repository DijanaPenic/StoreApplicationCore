using FluentValidation;

using Store.WebAPI.Models.Extensions;

namespace Store.WebAPI.Models.Identity
{
    public class ChangePasswordPutApiModel
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public bool SendMailNotification { get; set; }
    }

    public class ChangePasswordPutApiModelValidator : AbstractValidator<ChangePasswordPutApiModel>
    {
        public ChangePasswordPutApiModelValidator()
        {
            RuleFor(cp => cp.OldPassword);
            RuleFor(cp => cp.NewPassword).NotEmpty().Password();

            RuleFor(cp => cp.ConfirmPassword)
                .NotEmpty()
                .Equal(cp => cp.NewPassword).WithMessage("The new password and confirmation password must match.");
        }
    }
}