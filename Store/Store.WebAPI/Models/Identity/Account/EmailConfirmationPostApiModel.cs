using FluentValidation;

namespace Store.WebAPI.Models.Identity
{
    public class EmailConfirmationPostApiModel
    {
        public string ReturnUrl { get; set; }
    }

    public class EmailConfirmationPostApiModelValidator : AbstractValidator<EmailConfirmationPostApiModel>
    {
        public EmailConfirmationPostApiModelValidator()
        {
            RuleFor(emailConfirmation => emailConfirmation.ReturnUrl).NotEmpty();
        }
    }
}