using FluentValidation;

namespace Store.WebAPI.Models.Identity
{
    public class RegisterExternalPostApiModel
    {
        public string UserName { get; set; }

        public string AssociateEmail { get; set; }

        public bool AssociateExistingAccount { get; set; }

        public string ConfirmationUrl { get; set; }         
    }

    public class RegisterExternalPostApiModelValidator : AbstractValidator<RegisterExternalPostApiModel>
    {
        public RegisterExternalPostApiModelValidator()
        {
            RuleFor(registerExternal => registerExternal.AssociateExistingAccount).NotEmpty();
            RuleFor(registerExternal => registerExternal.ConfirmationUrl).NotEmpty();
        }
    }
}