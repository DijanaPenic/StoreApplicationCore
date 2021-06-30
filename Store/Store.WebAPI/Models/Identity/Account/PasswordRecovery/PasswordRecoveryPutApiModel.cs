using System;
using FluentValidation;

using Store.WebAPI.Models.Extensions;

namespace Store.WebAPI.Models.Identity
{
    public class PasswordRecoveryPutApiModel
    {
        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }

        public string PasswordRecoveryToken { get; set; }
        
        public Guid UserId { get; set; }
    }

    public class PasswordRecoveryPutApiModelValidator : AbstractValidator<PasswordRecoveryPutApiModel>
    {
        public PasswordRecoveryPutApiModelValidator()
        {
            RuleFor(passwordRecovery => passwordRecovery.NewPassword).NotEmpty().Password();

            RuleFor(passwordRecovery => passwordRecovery.ConfirmNewPassword)
                .NotEmpty()
                .Equal(passwordRecovery => passwordRecovery.NewPassword).WithMessage("The new password and confirmation password must match.");

            RuleFor(passwordRecovery => passwordRecovery.PasswordRecoveryToken).NotEmpty();
            RuleFor(passwordRecovery => passwordRecovery.UserId).NotEmpty();
        }
    }
}