using System.ComponentModel.DataAnnotations;

using Store.Common.Enums;

namespace Store.WebAPI.Models.Identity
{
    public class PolicyPostApiModel
    {
        [Required]
        public SectionType Section { get; set; }

        public AccessActionModel[] Actions { get; set; }
    }

    public class AccessActionModel
    {
        [Required]
        public AccessType Type { get; set; }

        public bool IsEnabled { get; set; }
    }
}