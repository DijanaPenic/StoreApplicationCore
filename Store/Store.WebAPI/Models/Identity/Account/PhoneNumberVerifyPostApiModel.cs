using FluentValidation;

using Store.WebAPI.Models.Extensions;

namespace Store.WebAPI.Models.Identity
{
    public class PhoneNumberVerifyPostApiModel
    {
        public string IsoCountryCode { get; set; }

        public string CountryCodeNumber { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsVoiceCall { get; set; }
    }

    public class PhoneNumberVerifyPostApiModelValidator : AbstractValidator<PhoneNumberVerifyPostApiModel>
    {
        public PhoneNumberVerifyPostApiModelValidator()
        {
            RuleFor(phoneNumberPassword => phoneNumberPassword.IsoCountryCode).NotEmpty();
            RuleFor(phoneNumberPassword => phoneNumberPassword.CountryCodeNumber).NotEmpty();
            RuleFor(phoneNumberPassword => phoneNumberPassword.PhoneNumber).NotEmpty().PhoneNumber();
        }
    }
}