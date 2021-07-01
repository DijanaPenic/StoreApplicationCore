using FluentValidation;

namespace Store.WebAPI.Models.Identity
{
    public class RolePostApiModel
    {
        public string Name { get; set; }

        public bool Stackable { get; set; }
    }
    
    public class RolePostApiModelValidator : AbstractValidator<RolePostApiModel>
    {
        public RolePostApiModelValidator()
        {
            RuleFor(r => r.Name).NotEmpty();
        }
    }
}