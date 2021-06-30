using FluentValidation;

using Store.Common.Enums;
using Store.WebAPI.Models.Extensions;

namespace Store.WebAPI.Models.Identity
{
    public class AccountVerificationPostApiModel
    {
        public AccountVerificationType Type { get; set; }
        
        public string ReturnUrl { get; set; }
        
        public string IsoCountryCode { get; set; }

        public string CountryCodeNumber { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsVoiceCall { get; set; }
    }

    public class AccountVerificationPostApiModelValidator : AbstractValidator<AccountVerificationPostApiModel>
    {
        public AccountVerificationPostApiModelValidator()
        { 
            RuleFor(accountVerification => accountVerification.Type).NotEmpty();
            
            When(accountVerification => accountVerification.Type == AccountVerificationType.PhoneNumber, () =>
            {
                RuleFor(accountVerification => accountVerification.IsoCountryCode).NotEmpty();
                RuleFor(accountVerification => accountVerification.CountryCodeNumber).NotEmpty();
                RuleFor(accountVerification => accountVerification.PhoneNumber).NotEmpty().PhoneNumber();
            }).Otherwise(() =>
            {
                RuleFor(accountVerification => accountVerification.ReturnUrl).NotEmpty();
            });
        }
    }
}