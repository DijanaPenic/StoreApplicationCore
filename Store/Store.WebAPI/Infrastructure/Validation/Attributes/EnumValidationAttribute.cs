using System;
using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Infrastructure.Validation.Attributes
{
    public class EnumValidationAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public EnumValidationAttribute(Type enumType)
        {
            _enumType = enumType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(Enum.TryParse(_enumType, value?.ToString(), true, out object result))
            {
                return ValidationResult.Success;
            }
            else
            {
                Lazy<ValidationResult> errorResult = new Lazy<ValidationResult>(() => new ValidationResult("Enum validation failed", new string[] { validationContext.MemberName }));
                return errorResult.Value;
            }
        }
    }
}