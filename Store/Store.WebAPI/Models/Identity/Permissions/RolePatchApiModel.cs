using FluentValidation;

namespace Store.WebAPI.Models.Identity
{
    public class RolePatchApiModel
    {
        public string Name { get; set; }

        public bool Stackable { get; set; }
    }
    
    public class RolePatchApiModelValidator : AbstractValidator<RolePatchApiModel>
    {
        public RolePatchApiModelValidator()
        {
            RuleFor(r => r.Name).NotEmpty();
        }
    }
}