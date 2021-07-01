using FluentValidation;

using Store.WebAPI.Models.Extensions;

namespace Store.WebAPI.Models.Identity
{
    public class UserPostApiModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public bool IsApproved { get; set; }

        public bool LockoutEnabled { get; set; }

        public string[] Roles { get; set; }
    }

    public class UserPostApiModelValidator : AbstractValidator<UserPostApiModel>
    {
        public UserPostApiModelValidator()
        {
            RuleFor(u => u.UserName).NotEmpty();
            RuleFor(u => u.Email).NotEmpty().EmailAddress();
            RuleFor(u => u.PhoneNumber).PhoneNumber();
            RuleFor(u => u.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(u => u.LastName).NotEmpty().MaximumLength(50);
            RuleFor(u => u.Password).NotEmpty().Password();

            RuleFor(u => u.ConfirmPassword)
                .NotEmpty()
                .Equal(u => u.Password).WithMessage("Passwords must match.");
        }
    }
}