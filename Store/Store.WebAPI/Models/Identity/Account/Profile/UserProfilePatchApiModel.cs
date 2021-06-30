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
            RuleFor(userProfile => userProfile.Email).NotEmpty().EmailAddress();
            RuleFor(userProfile => userProfile.ConfirmationUrl).NotEmpty();
            RuleFor(userProfile => userProfile.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(userProfile => userProfile.LastName).NotEmpty().MaximumLength(50);
        }
    }
}