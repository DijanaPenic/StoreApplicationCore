using FluentValidation;

namespace Store.WebAPI.Models.Extensions
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Length(8, 100)
                .Matches("[A-Z]").WithMessage("Password must contain a upper case ASCII character.")
                .Matches("[a-z]").WithMessage("Password must contain a lower case ASCII character.")
                .Matches("[0-9]").WithMessage("Password must contain a digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a non-alphanumeric character.");
        }

        public static IRuleBuilder<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Matches(@"^(?:(?:\(?(?:00|\+)([1-4]\d\d|[1-9]\d?)\)?)?[\-\.\ \\\/]?)?((?:\(?\d{1,}\)?[\-\.\ \\\/]?){0,})(?:[\-\.\ \\\/]?(?:#|ext\.?|extension|x)[\-\.\ \\\/]?(\d+))?$")
                .WithMessage("Phone Number is not a valid phone number.");
        }
    }
}