using FluentValidation;

using Store.Common.Enums;
using Store.WebAPI.Models.Extensions;

namespace Store.WebAPI.Models.Identity
{
    public class AccountVerificationPutApiModel
    {
        public AccountVerificationType Type { get; set; }

        public string Token { get; set; }
        
        public string PhoneNumber { get; set; }
    }

    public class AccountVerificationPutApiModelValidator : AbstractValidator<AccountVerificationPutApiModel>
    {
        public AccountVerificationPutApiModelValidator()
        {
            RuleFor(accountVerification => accountVerification.Type).NotEmpty();
            RuleFor(accountVerification => accountVerification.Token).NotEmpty();
            
            When(accountVerification => accountVerification.Type == AccountVerificationType.PhoneNumber, () =>
            {
                RuleFor(accountVerification => accountVerification.PhoneNumber).NotEmpty().PhoneNumber();
            });
        }
    }
}