using System.ComponentModel.DataAnnotations;

using Store.Common.Enums;
using Store.WebAPI.Infrastructure.Validation.Attributes;

namespace Store.WebAPI.Models.Identity
{
    public class PolicyPostApiModel
    {
        [Required]
        [EnumValidation(typeof(SectionType))]
        public string Section { get; set; }

        public AccessActionModel[] AccessActions { get; set; }
    }

    public class AccessActionModel
    {
        [Required]
        [EnumValidation(typeof(AccessType))]
        public string Type { get; set; }

        public bool IsEnabled { get; set; }
    }
}