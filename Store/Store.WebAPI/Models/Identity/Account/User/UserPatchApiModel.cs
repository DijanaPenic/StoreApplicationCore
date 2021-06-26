using FluentValidation;

using Store.WebAPI.Models.Extensions;

namespace Store.WebAPI.Models.Identity
{
    public class UserPatchApiModel
    {
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsApproved { get; set; }

        public string[] Roles { get; set; }
    }

    public class UserPatchApiModelValidator : AbstractValidator<UserPatchApiModel>
    {
        public UserPatchApiModelValidator()
        {
            RuleFor(user => user.Email).NotEmpty().EmailAddress();
            RuleFor(user => user.PhoneNumber).PhoneNumber();
            RuleFor(user => user.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(user => user.LastName).NotEmpty().MaximumLength(50);
        }
    }
}