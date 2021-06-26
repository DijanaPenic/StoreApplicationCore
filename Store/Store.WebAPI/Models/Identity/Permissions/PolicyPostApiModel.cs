using FluentValidation;

using Store.Common.Enums;

namespace Store.WebAPI.Models.Identity
{
    public class PolicyPostApiModel
    {
        public SectionType Section { get; set; }

        public AccessActionModel[] Actions { get; set; }
    }

    public class AccessActionModel
    {
        public AccessType Type { get; set; }

        public bool IsEnabled { get; set; }
    }

    public class PolicyPostApiModelValidator : AbstractValidator<PolicyPostApiModel>
    {
        public PolicyPostApiModelValidator()
        {
            RuleFor(policy => policy.Section).IsInEnum();
            
            RuleForEach(policy => policy.Actions).ChildRules(inlineValidator => {
                inlineValidator.RuleFor(action => action.Type).IsInEnum();
            });
        }
    }
}