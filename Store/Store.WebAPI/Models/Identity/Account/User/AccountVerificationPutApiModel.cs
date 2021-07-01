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
            RuleFor(av => av.Type).NotEmpty();
            RuleFor(av => av.Token).NotEmpty();
            
            When(av => av.Type == AccountVerificationType.PhoneNumber, () =>
            {
                RuleFor(av => av.PhoneNumber).NotEmpty().PhoneNumber();
            });
        }
    }
}