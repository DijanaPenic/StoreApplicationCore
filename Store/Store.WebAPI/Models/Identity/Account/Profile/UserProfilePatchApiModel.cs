using FluentValidation;

namespace Store.WebAPI.Models.Identity
{
    public class UserProfilePatchApiModel
    {
        public string Email { get; set; }

        public string ConfirmationUrl { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class UserProfilePatchApiModelValidator : AbstractValidator<UserProfilePatchApiModel>
    {
        public UserProfilePatchApiModelValidator()
        {
            RuleFor(up => up.Email).NotEmpty().EmailAddress();
            RuleFor(up => up.ConfirmationUrl).NotEmpty();
            RuleFor(up => up.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(up => up.LastName).NotEmpty().MaximumLength(50);
        }
    }
}