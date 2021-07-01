using FluentValidation;

using Store.Common.Enums;

namespace Store.WebAPI.Models.Identity
{
    public class AccessActionPostModel
    {
        public AccessType Type { get; set; }

        public bool IsEnabled { get; set; }
    }

    public class AccessActionModelValidator : AbstractValidator<AccessActionPostModel>
    {
        public AccessActionModelValidator()
        {
            RuleFor(a => a.Type).NotEmpty();
        }
    }
}