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
            RuleFor(user => user.UserName).NotEmpty();
            RuleFor(user => user.Email).NotEmpty().EmailAddress();
            RuleFor(user => user.PhoneNumber).PhoneNumber();
            RuleFor(user => user.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(user => user.LastName).NotEmpty().MaximumLength(50);
            RuleFor(user => user.Password).NotEmpty().Password();

            RuleFor(user => user.ConfirmPassword)
                .NotEmpty()
                .Equal(user => user.Password).WithMessage("Passwords must match.");
        }
    }
}