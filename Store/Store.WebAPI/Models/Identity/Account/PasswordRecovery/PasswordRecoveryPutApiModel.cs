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
            RuleFor(pr => pr.NewPassword).NotEmpty().Password();

            RuleFor(pr => pr.ConfirmNewPassword)
                .NotEmpty()
                .Equal(pr => pr.NewPassword).WithMessage("The new password and confirmation password must match.");

            RuleFor(pr => pr.PasswordRecoveryToken).NotEmpty();
            RuleFor(pr => pr.UserId).NotEmpty();
        }
    }
}