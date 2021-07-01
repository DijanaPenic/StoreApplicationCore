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
            RuleFor(av => av.Type).NotEmpty();
            
            When(av => av.Type == AccountVerificationType.PhoneNumber, () =>
            {
                RuleFor(av => av.IsoCountryCode).NotEmpty();
                RuleFor(av => av.CountryCodeNumber).NotEmpty();
                RuleFor(av => av.PhoneNumber).NotEmpty().PhoneNumber();
            }).Otherwise(() =>
            {
                RuleFor(av => av.ReturnUrl).NotEmpty();
            });
        }
    }
}